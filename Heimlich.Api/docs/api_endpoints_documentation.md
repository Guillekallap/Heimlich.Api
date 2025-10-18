# Heimlich API - Documentación de Endpoints

## Autenticación

### Registrar usuario
- **URL:** `/api/auth/register`
- **Método:** POST
- **Body:**
```json
{
  "userName": "nuevoUsuario",
  "fullName": "Nombre Apellido",
  "email": "mail@dominio.com",
  "password": "TuPassword123",
  "role": 2
}
```
- **Respuesta:** 201 Created, datos del usuario registrado.

---

### Login
- **URL:** `/api/auth/login`
- **Método:** POST
- **Body:**
```json
{
  "userName": "nuevoUsuario",
  "password": "TuPassword123"
}
```
- **Respuesta:** 200 OK, token JWT y datos del usuario.

---

### Logout
- **URL:** `/api/auth/logout`
- **Método:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:** (vacío)
- **Respuesta:** 200 OK, mensaje de logout.

---

### Perfil de usuario
- **URL:** `/api/auth/profile?userId={id}`
- **Método:** GET
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 200 OK, datos del usuario.

---

### Cambiar contraseña
- **URL:** `/api/auth/change-password`
- **Método:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "currentPassword": "TuPassword123",
  "newPassword": "NuevoPassword456"
}
```
- **Respuesta:** 200 OK, mensaje de éxito.

---

## Grupos

### Crear grupo
- **URL:** `/api/instructor/groups`
- **Método:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "name": "Grupo A",
  "description": "Entrenamiento inicial",
  "practitionerIds": ["USER_ID_1", "USER_ID_2"],
  "evaluationConfigId": 1
}
```
- **Respuesta:** 200 OK, datos del grupo creado.

---

### Obtener grupos del instructor
- **URL:** `/api/instructor/groups/owned`
- **Método:** GET
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 200 OK, lista de grupos.

---

### Editar grupo
- **URL:** `/api/instructor/groups/{groupId}`
- **Método:** PUT
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "name": "Grupo A Editado",
  "description": "Nueva descripción",
  "practitionerIds": ["USER_ID_1", "USER_ID_3"]
}
```
- **Respuesta:** 200 OK, grupo editado.

---

### Eliminar grupo
- **URL:** `/api/instructor/groups/{id}`
- **Método:** DELETE
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 204 No Content

---

### Asignar configuración de evaluación a grupo
- **URL:** `/api/instructor/groups/{groupId}/config/{configId}`
- **Método:** POST
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 200 OK, configuración vinculada.

---

### Visualizar grupos asignados a practicante
- **URL:** `/api/instructor/groups/assigned?userId={userId}`
- **Método:** GET
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 200 OK, datos del grupo asignado al practicante.

---

## Evaluaciones

### Crear evaluación
- **URL:** `/api/instructor/evaluations/create`
- **Método:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "evaluatedUserId": "USER_ID_1",
  "trunkId": 1,
  "groupId": 1,
  "comments": "Evaluación con muchas métricas",
  "score": 90,
  "measurements": [
    {
      "elapsedMs": 10000,
      "forceValue": "30",
      "forceIsValid": true,
      "touchValue": "1",
      "touchIsValid": true,
      "handPositionValue": "Centered",
      "handPositionIsValid": true,
      "positionValue": "Upright",
      "positionIsValid": false,
      "isValid": false
    }
    // ...más mediciones
  ]
}
```
- **Respuesta:** 200 OK, datos de la evaluación creada.

---

### Validar evaluación
- **URL:** `/api/instructor/evaluations/{evaluationId}/validate`
- **Método:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "score": 95,
  "isValid": true,
  "comments": "Excelente",
  "signature": "firmaInstructor",
  "evaluationConfigId": 1
}
```
- **Respuesta:** 200 OK, evaluación validada.

---

### Cancelar evaluación
- **URL:** `/api/instructor/evaluations/cancel`
- **Método:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:** igual a crear evaluación.
- **Respuesta:** 200 OK, evaluación cancelada.

---

### Obtener todas las evaluaciones del instructor
- **URL:** `/api/instructor/evaluations`
- **Método:** GET
- **Headers:** Authorization: Bearer {token}
- **Descripción:** Devuelve todas las evaluaciones creadas por el instructor autenticado. Puede incluir muchas evaluaciones de distintos grupos y practicantes.
- **Ejemplo de response:**
```json
{
  "id": 1,
  "evaluatorId": "INSTRUCTOR_ID",
  "evaluatedUserId": "PRACTICANTE_ID",
  "trunkId": 1,
  "groupId": 2,
  "evaluationConfigId": 1,
  "score": 90,
  "comments": "Buen desempeño",
  "isValid": true,
  "state": "Validated",
  "totalErrors": 1,
  "totalSuccess": 3,
  "totalMeasurements": 4,
  "successRate": 0.75,
  "measurements": [
    {
      "elapsedMs": 10000,
      "forceValue": "30",
      "forceIsValid": true,
      "touchValue": "1",
      "touchIsValid": true,
      "handPositionValue": "Centered",
      "handPositionIsValid": true,
      "positionValue": "Upright",
      "positionIsValid": false,
      "isValid": false
    }
    // ...más mediciones
  ]
}
```

---

### Obtener evaluaciones por grupo y practicante
- **URL:** `/api/instructor/evaluations/by-group-practitioner?groupId={groupId}&userId={userId}`
- **Método:** GET
- **Headers:** Authorization: Bearer {token}
- **Descripción:** Devuelve solo las evaluaciones de un practicante específico dentro de un grupo. Útil para mostrar el historial de un usuario en un grupo concreto.
- **Ejemplo de response:**
```json
{
  "id": 2,
  "evaluatorId": "INSTRUCTOR_ID",
  "evaluatedUserId": "PRACTICANTE_ID",
  "trunkId": 1,
  "groupId": 2,
  "evaluationConfigId": 1,
  "score": 85,
  "comments": "Mejorable",
  "isValid": true,
  "state": "Validated",
  "totalErrors": 2,
  "totalSuccess": 2,
  "totalMeasurements": 4,
  "successRate": 0.5,
  "measurements": [
    {
      "elapsedMs": 10000,
      "forceValue": "28",
      "forceIsValid": true,
      "touchValue": "0",
      "touchIsValid": false,
      "handPositionValue": "Left",
      "handPositionIsValid": true,
      "positionValue": "Upright",
      "positionIsValid": true,
      "isValid": false
    }
    // ...más mediciones
  ]
}
```

---

## Configuraciones de Evaluación

### Crear configuración
- **URL:** `/api/instructor/evaluation-configs`
- **Método:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "name": "Config Avanzada",
  "maxErrors": 5,
  "maxTime": 60,
  "maxTimeInterval": 120,
  "sensorIntervals": []
}
```
- **Respuesta:** 200 OK, configuración creada.

---

### Obtener configuraciones de evaluación
- **URL:** `/api/instructor/evaluation-configs`
- **Método:** GET
- **Headers:** Authorization: Bearer {token}
- **Descripción:** Devuelve todas las configuraciones de evaluación disponibles. Incluye el campo `maxTimeInterval` para definir el lapso en segundos entre evaluaciones automáticas.
- **Ejemplo de response:**
```json
{
  "id": 1,
  "name": "Config Default",
  "maxErrors": 10,
  "maxTime": 30,
  "maxTimeInterval": 60,
  "isDefault": true,
  "creationDate": "2024-06-01T12:00:00Z"
},
{
  "id": 2,
  "name": "Config Avanzada",
  "maxErrors": 5,
  "maxTime": 60,
  "maxTimeInterval": 120,
  "isDefault": false,
  "creationDate": "2024-06-01T12:05:00Z"
}
```

---

### Editar configuración
- **URL:** `/api/instructor/evaluation-configs/{id}`
- **Método:** PUT
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "name": "Config Editada",
  "maxErrors": 7,
  "maxTime": 45,
  "maxTimeInterval": 90,
  "sensorIntervals": []
}
```
- **Respuesta:** 200 OK, configuración editada.

---

### Eliminar configuración
- **URL:** `/api/instructor/evaluation-configs/{id}`
- **Método:** DELETE
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 204 No Content

---

## Simulaciones

### Crear simulación
- **URL:** `/api/practitioner/simulations/create`
- **Método:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "trunkId": 1,
  "comments": "Simulación extensa",
  "samples": [
    {
      "elapsedMs": 10000,
      "measurement": {
        "forceValue": "30",
        "forceIsValid": true,
        "touchValue": "1",
        "touchIsValid": true,
        "handPositionValue": "Centered",
        "handPositionIsValid": true,
        "positionValue": "Upright",
        "positionIsValid": false,
        "isValid": false
      }
    }
    // ...más samples
  ]
}
```
- **Respuesta:** 200 OK, datos de la simulación creada.

---

### Obtener simulaciones del practicante
- **URL:** `/api/practitioner/simulations`
- **Método:** GET
- **Headers:** Authorization: Bearer {token}
- **Descripción:** Devuelve todas las simulaciones realizadas por el practicante autenticado, incluyendo todas las mediciones y estadísticas.
- **Ejemplo de response:**
```json
{
  "id": 1,
  "practitionerId": "PRACTICANTE_ID",
  "trunkId": 1,
  "totalDurationMs": 30000,
  "totalErrors": 1,
  "totalSuccess": 3,
  "totalMeasurements": 4,
  "successRate": 0.75,
  "averageErrorsPerMeasurement": 0.25,
  "isValid": true,
  "comments": "Simulación completa",
  "samples": [
    {
      "elapsedMs": 10000,
      "measurement": {
        "forceValue": "30",
        "forceIsValid": true,
        "touchValue": "1",
        "touchIsValid": true,
        "handPositionValue": "Centered",
        "handPositionIsValid": true,
        "positionValue": "Upright",
        "positionIsValid": false,
        "isValid": false
      }
    }
    // ...más samples
  ]
}
```

---

### Cancelar simulación
- **URL:** `/api/practitioner/simulations/cancel`
- **Método:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:** igual a crear simulación.
- **Respuesta:** 200 OK, simulación cancelada.

---

**Notas:**
- Los endpoints de consulta permiten filtrar y obtener solo la información relevante para el usuario o grupo.
- Los campos obligatorios y opcionales están reflejados en los ejemplos de response.
- El campo `maxTimeInterval` en las configuraciones de evaluación se usa para definir el lapso (en segundos) entre evaluaciones automáticas en el grupo.
- Todos los endpoints requieren JWT salvo los de registro y login.
- Puedes consultar los endpoints en Swagger si lo tienes habilitado en desarrollo.
