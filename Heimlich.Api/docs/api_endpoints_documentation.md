# Heimlich API - Documentaci�n de Endpoints

## Autenticaci�n

### Registrar usuario
- **URL:** `/api/auth/register`
- **M�todo:** POST
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
- **M�todo:** POST
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
- **M�todo:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:** (vac�o)
- **Respuesta:** 200 OK, mensaje de logout.

---

### Perfil de usuario
- **URL:** `/api/auth/profile?userId={id}`
- **M�todo:** GET
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 200 OK, datos del usuario.

---

### Cambiar contrase�a
- **URL:** `/api/auth/change-password`
- **M�todo:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "currentPassword": "TuPassword123",
  "newPassword": "NuevoPassword456"
}
```
- **Respuesta:** 200 OK, mensaje de �xito.

---

## Grupos

### Crear grupo
- **URL:** `/api/instructor/groups`
- **M�todo:** POST
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
- **M�todo:** GET
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 200 OK, lista de grupos.

---

### Editar grupo
- **URL:** `/api/instructor/groups/{groupId}`
- **M�todo:** PUT
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "name": "Grupo A Editado",
  "description": "Nueva descripci�n",
  "practitionerIds": ["USER_ID_1", "USER_ID_3"]
}
```
- **Respuesta:** 200 OK, grupo editado.

---

### Eliminar grupo
- **URL:** `/api/instructor/groups/{id}`
- **M�todo:** DELETE
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 204 No Content

---

### Asignar configuraci�n de evaluaci�n a grupo
- **URL:** `/api/instructor/groups/{groupId}/config/{configId}`
- **M�todo:** POST
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 200 OK, configuraci�n vinculada.

---

### Visualizar grupos asignados a practicante
- **URL:** `/api/instructor/groups/assigned?userId={userId}`
- **M�todo:** GET
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 200 OK, datos del grupo asignado al practicante.

---

## Evaluaciones

### Crear evaluaci�n
- **URL:** `/api/instructor/evaluations/create`
- **M�todo:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "evaluatedUserId": "USER_ID_1",
  "trunkId": 1,
  "groupId": 1,
  "comments": "Evaluaci�n con muchas m�tricas",
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
    // ...m�s mediciones
  ]
}
```
- **Respuesta:** 200 OK, datos de la evaluaci�n creada.

---

### Validar evaluaci�n
- **URL:** `/api/instructor/evaluations/{evaluationId}/validate`
- **M�todo:** POST
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
- **Respuesta:** 200 OK, evaluaci�n validada.

---

### Cancelar evaluaci�n
- **URL:** `/api/instructor/evaluations/cancel`
- **M�todo:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:** igual a crear evaluaci�n.
- **Respuesta:** 200 OK, evaluaci�n cancelada.

---

### Obtener todas las evaluaciones del instructor
- **URL:** `/api/instructor/evaluations`
- **M�todo:** GET
- **Headers:** Authorization: Bearer {token}
- **Descripci�n:** Devuelve todas las evaluaciones creadas por el instructor autenticado. Puede incluir muchas evaluaciones de distintos grupos y practicantes.
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
  "comments": "Buen desempe�o",
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
    // ...m�s mediciones
  ]
}
```

---

### Obtener evaluaciones por grupo y practicante
- **URL:** `/api/instructor/evaluations/by-group-practitioner?groupId={groupId}&userId={userId}`
- **M�todo:** GET
- **Headers:** Authorization: Bearer {token}
- **Descripci�n:** Devuelve solo las evaluaciones de un practicante espec�fico dentro de un grupo. �til para mostrar el historial de un usuario en un grupo concreto.
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
    // ...m�s mediciones
  ]
}
```

---

## Configuraciones de Evaluaci�n

### Crear configuraci�n
- **URL:** `/api/instructor/evaluation-configs`
- **M�todo:** POST
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
- **Respuesta:** 200 OK, configuraci�n creada.

---

### Obtener configuraciones de evaluaci�n
- **URL:** `/api/instructor/evaluation-configs`
- **M�todo:** GET
- **Headers:** Authorization: Bearer {token}
- **Descripci�n:** Devuelve todas las configuraciones de evaluaci�n disponibles. Incluye el campo `maxTimeInterval` para definir el lapso en segundos entre evaluaciones autom�ticas.
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

### Editar configuraci�n
- **URL:** `/api/instructor/evaluation-configs/{id}`
- **M�todo:** PUT
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
- **Respuesta:** 200 OK, configuraci�n editada.

---

### Eliminar configuraci�n
- **URL:** `/api/instructor/evaluation-configs/{id}`
- **M�todo:** DELETE
- **Headers:** Authorization: Bearer {token}
- **Respuesta:** 204 No Content

---

## Simulaciones

### Crear simulaci�n
- **URL:** `/api/practitioner/simulations/create`
- **M�todo:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:**
```json
{
  "trunkId": 1,
  "comments": "Simulaci�n extensa",
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
    // ...m�s samples
  ]
}
```
- **Respuesta:** 200 OK, datos de la simulaci�n creada.

---

### Obtener simulaciones del practicante
- **URL:** `/api/practitioner/simulations`
- **M�todo:** GET
- **Headers:** Authorization: Bearer {token}
- **Descripci�n:** Devuelve todas las simulaciones realizadas por el practicante autenticado, incluyendo todas las mediciones y estad�sticas.
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
  "comments": "Simulaci�n completa",
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
    // ...m�s samples
  ]
}
```

---

### Cancelar simulaci�n
- **URL:** `/api/practitioner/simulations/cancel`
- **M�todo:** POST
- **Headers:** Authorization: Bearer {token}
- **Body:** igual a crear simulaci�n.
- **Respuesta:** 200 OK, simulaci�n cancelada.

---

**Notas:**
- Los endpoints de consulta permiten filtrar y obtener solo la informaci�n relevante para el usuario o grupo.
- Los campos obligatorios y opcionales est�n reflejados en los ejemplos de response.
- El campo `maxTimeInterval` en las configuraciones de evaluaci�n se usa para definir el lapso (en segundos) entre evaluaciones autom�ticas en el grupo.
- Todos los endpoints requieren JWT salvo los de registro y login.
- Puedes consultar los endpoints en Swagger si lo tienes habilitado en desarrollo.
