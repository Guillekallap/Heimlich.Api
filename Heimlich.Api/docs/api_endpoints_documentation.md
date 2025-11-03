# Heimlich API - Documentación de Endpoints

Base URL (producción)
- https://heimlich-api-unlam.azurewebsites.net

Encabezados comunes
- `Content-Type: application/json`
- `Authorization: Bearer {token}` (para endpoints protegidos)

Token JWT
- El endpoint de login devuelve un JWT en el cuerpo. El campo puede llamarse `token` o `access_token` dependiendo de la versión del cliente. Usa ese valor en el header `Authorization: Bearer {token}`.
- Nota de configuración: la clave JWT (`Jwt__Key`) y valores `Jwt__Issuer` / `Jwt__Audience` se configuran en App Service -> Configuration (o en Key Vault) en producción. Asegúrate de que la app móvil use el mismo `Audience` que está configurado en la API.

Cómo probar rápidamente
- Postman: POST al endpoint de login, copiar token y usar en Authorization -> Bearer Token.
- curl ejemplo:
  - Obtener token:
    `curl -X POST -H "Content-Type: application/json" -d '{"userName":"tuUsuario","password":"tuPass"}' https://heimlich-api-unlam.azurewebsites.net/api/auth/login`
  - Llamar endpoint protegido:
    `curl -H "Authorization: Bearer <TOKEN>" https://heimlich-api-unlam.azurewebsites.net/api/instructor/evaluations`
- React Native (fetch + AsyncStorage):
  - Guardar token tras login y añadir `Authorization` en las fetch posteriores. (Ver ejemplos al final del documento.)

Nota sobre CORS
- Aplicaciones nativas (React Native Android/iOS) no requieren configuración CORS.
- Aplicaciones web/SPA deben tener su origen añadido en la política CORS del backend. En producción el origen esperado es `https://heimlich-app-mobile.azurestaticapps.net`.
- Si necesitas otro origen, pedir que lo agreguen en App Service -> Configuration o modificar la política CORS en `Program.cs`.

Swagger
- Swagger está deshabilitado por defecto en producción (`EnableSwagger=false`). Para ver la documentación en desarrollo activa `EnableSwagger=true` en configuración o revisa localmente con `dotnet run`.

Application Insights
- Application Insights está habilitado en producción y la cadena de conexión se encuentra en App Service -> Configuration como `APPLICATIONINSIGHTS_CONNECTION_STRING`.
- Puedes usar `Live Metrics` para ver peticiones en tiempo real y `Failures` para revisar excepciones.

Autenticación
- Todos los endpoints requieren JWT salvo `/api/auth/register` y `/api/auth/login`.
- Asegúrate de usar el `Issuer` y `Audience` configurados en la app para validar tokens.

Notas sobre despliegue y base de datos
- En producción la aplicación usa la cadena `DefaultConnection` configurada en App Service -> Connection strings. En este proyecto hemos configurado la aplicación para soportar autenticación mediante Managed Identity (Azure AD) usando `Authentication=Active Directory Default` en la cadena de conexión.
- Para que la API pueda crear las tablas mediante las migraciones al arrancar, la Managed Identity del App Service debe existir y tener permisos en la base de datos (p.ej. `db_owner` temporal o roles `db_ddladmin`/`db_datareader`/`db_datawriter`).
- Por seguridad, la ejecución automática de migraciones está deshabilitada por defecto; puede activarse temporalmente con `ApplyMigrationsOnStartup=true` en App Service -> Configuration.

---

# Endpoints

## Autenticación

### Registrar usuario
- URL: `/api/auth/register`
- Método: `POST`
- Body:
```json
{
  "userName": "nuevoUsuario",
  "fullName": "Nombre Apellido",
  "email": "mail@dominio.com",
  "password": "TuPassword123",
  "role": 2
}
```
- Respuesta: `201 Created` (datos del usuario). Ejemplo de respuesta:
```json
{
  "id": "GUID-DEL-USUARIO",
  "userName": "nuevoUsuario",
  "email": "mail@dominio.com",
  "fullName": "Nombre Apellido",
  "roles": [2]
}
```

---

### Login
- URL: `/api/auth/login`
- Método: `POST`
- Body:
```json
{
  "userName": "nuevoUsuario",
  "password": "TuPassword123"
}
```
- Respuesta: `200 OK` con token JWT en el body. Ejemplo:
```json
{
  "token": "eyJhbGci...",
  "expiresIn": 3600,
  "user": { "id": "GUID-DEL-USUARIO", "userName": "nuevoUsuario" }
}
```

---

### Logout
- URL: `/api/auth/logout`
- Método: `POST`
- Headers: `Authorization: Bearer {token}`
- Body: vacío
- Respuesta: `200 OK`. Ejemplo:
```json
{ "message": "Logout successful" }
```

---

### Perfil de usuario
- URL: `/api/auth/profile?userId={id}`  
- Método: `GET`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `200 OK`, datos del usuario. Ejemplo:
```json
{
  "id": "GUID-DEL-USUARIO",
  "userName": "nuevoUsuario",
  "fullName": "Nombre Apellido",
  "email": "mail@dominio.com",
  "roles": [2]
}
```

---

### Cambiar contraseña
- URL: `/api/auth/change-password`
- Método: `POST`
- Headers: `Authorization: Bearer {token}`
- Body:
```json
{
  "currentPassword": "TuPassword123",
  "newPassword": "NuevoPassword456"
}
```
- Respuesta: `200 OK`. Ejemplo:
```json
{ "message": "Password changed successfully" }
```

---

## Grupos (Instructor)

### Crear grupo
- URL: `/api/instructor/groups`
- Método: `POST`
- Headers: `Authorization: Bearer {token}`
- Body:
```json
{
  "name": "Grupo A",
  "description": "Entrenamiento inicial",
  "evaluationDate": "2025-02-15T10:00:00Z",
  "practitionerIds": ["USER_ID_1", "USER_ID_2"],
  "evaluationConfigId": 1
}
```
- **Nota:** `evaluationDate` es **obligatorio** - fecha en que se programará la evaluación del grupo.
- Respuesta: `200 OK`, datos del grupo creado. Ejemplo:
```json
{
  "id": 123,
  "name": "Grupo A",
  "description": "Entrenamiento inicial",
  "creationDate": "2025-01-24T20:00:00Z",
  "evaluationDate": "2025-02-15T10:00:00Z",
  "status": "Active",
  "ownerInstructorId": "INSTRUCTOR_ID",
  "ownerInstructorName": "Juan Pérez",
  "practitionerIds": ["USER_ID_1", "USER_ID_2"]
}
```

---

### Obtener grupos del instructor
- URL: `/api/instructor/groups/owned`
- Método: `GET`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `200 OK`. Ejemplo:
```json
[
  {
    "id": 123,
    "name": "Grupo A",
    "description": "Entrenamiento inicial",
    "creationDate": "2025-01-24T20:00:00Z",
    "evaluationDate": "2025-02-15T10:00:00Z",
    "status": "Active",
    "ownerInstructorId": "INSTRUCTOR_ID",
    "ownerInstructorName": "Juan Pérez",
    "practitionerIds": ["USER_ID_1", "USER_ID_2"]
  }
]
```

---

### Editar grupo
- URL: `/api/instructor/groups/{groupId}`
- Método: `PUT`
- Headers: `Authorization: Bearer {token}`
- Body:
```json
{
  "name": "Grupo A Editado",
  "description": "Nueva descripción",
  "evaluationDate": "2025-02-20T14:00:00Z",
  "practitionerIds": ["USER_ID_1", "USER_ID_3"]
}
```
- **Nota:** `evaluationDate` es **obligatorio**.
- Respuesta: `200 OK`. Ejemplo:
```json
{
  "id": 123,
  "name": "Grupo A Editado",
  "description": "Nueva descripción",
  "creationDate": "2025-01-24T20:00:00Z",
  "evaluationDate": "2025-02-20T14:00:00Z",
  "status": "Active",
  "ownerInstructorId": "INSTRUCTOR_ID",
  "ownerInstructorName": "Juan Pérez",
  "practitionerIds": ["USER_ID_1", "USER_ID_3"]
}
```

---

### Eliminar grupo
- URL: `/api/instructor/groups/{id}`
- Método: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `204 No Content`.


---

### Asignar configuración de evaluación a grupo
- URL: `/api/instructor/groups/{groupId}/config/{configId}`
- Método: `POST`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `200 OK`, configuración vinculada. Ejemplo:
```json
{ "message": "Configuration assigned" }
```

---

### Visualizar grupos asignados a practicante
- URL: `/api/instructor/groups/assigned?userId={userId}`
- Método: `GET`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `200 OK`, datos del grupo asignado al practicante. Ejemplo:
```json
[
  {
    "id": 123,
    "name": "Grupo A",
    "evaluationDate": "2025-02-15T10:00:00Z",
    "assignedTo": "PRACTICANTE_ID"
  }
]
```

---

## Evaluaciones (Instructor)

### Crear evaluación
- URL: `/api/instructor/evaluations/create`
- Método: `POST`
- Headers:
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`
- Body (completo, ejemplo con 10 mediciones y agregados):
```json
{
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "trunkId": 1,
  "groupId": 10,
  "comments": "Evaluación inicial - prueba Postman con 10 mediciones.",
  "score": 70,
  "measurements": [
    {
      "timestamp": 1698140000000,
      "elapsedMs": 10000,
      "result": "OK",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "32",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    }
  ],
  "totalDurationMs": 145000,
  "totalMeasurements": 10,
  "totalSuccess": 7,
  "totalErrors": 3,
  "successRate": 0.7,
  "averageErrorsPerMeasurement": 0.3
}
```
- Respuesta esperada (ejemplo): `200 OK` con el objeto de la evaluación creada. Ejemplo de response:
```json
{
  "id": 456,
  "evaluatorId": "INSTRUCTOR_GUID_1",
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "evaluatedUserFullName": "María González",
  "trunkId": 1,
  "groupId": 10,
  "evaluationConfigId": 2,
  "score": 70,
  "comments": "Evaluación inicial - prueba Postman con 10 mediciones...",
  "state": "Active",
  "creationDate": "2025-01-24T21:00:00Z",
  "validatedAt": null,
  "totalErrors": 3,
  "totalSuccess": 7,
  "totalMeasurements": 10,
  "successRate": 0.7,
  "totalDurationMs": 145000,
  "averageErrorsPerMeasurement": 0.3,
  "measurements": [ /* objetos de medición */ ]
}
```
- **Nota:** Ya no se incluye el campo `isValid` en la respuesta (fue eliminado de la entidad).

---

### Validar evaluación
- URL: `/api/instructor/evaluations/{evaluationId}/validate`
- Método: `POST`
- Headers:
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`
- Body (raw JSON):
```json
{
  "score": 92,
  "comments": "Validación final: cumple con los requisitos",
  "signature": "firmaInstructorEjemploBase64==",
  "evaluationConfigId": 1
}
```
- **Nota:** El campo `is_valid` fue eliminado del body (ya no se usa).
- Respuesta esperada: `200 OK` con el objeto evaluado actualizado. Ejemplo:
```json
{
  "id": 456,
  "evaluatorId": "INSTRUCTOR_GUID_1",
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "evaluatedUserFullName": "María González",
  "score": 92,
  "state": "Validated",
  "validatedAt": "2025-01-24T21:30:00Z",
  "totalErrors": 3,
  "totalSuccess": 7,
  "totalMeasurements": 10,
  "successRate": 0.7,
  "measurements": []
}
```

### Cancelar evaluación
- URL: `/api/instructor/evaluations/cancel`
- Método: `POST`
- Headers: `Authorization: Bearer {token}`
- Body: mismo formato que crear evaluación (completo)
- Respuesta: `200 OK`, evaluación cancelada. Ejemplo:
```json
{ "message": "Evaluation canceled" }
```

---

### Obtener todas las evaluaciones del instructor
- URL: `/api/instructor/evaluations`
- Método: `GET`
- Headers: `Authorization: Bearer {token}`
- Descripción: Devuelve todas las evaluaciones creadas por el instructor autenticado.
- Ejemplo de response:
```json
[
  {
    "id": 1,
    "evaluatorId": "INSTRUCTOR_ID",
    "evaluatedUserId": "PRACTICANTE_ID",
    "evaluatedUserFullName": "Carlos Ruiz",
    "trunkId": 1,
    "groupId": 2,
    "evaluationConfigId": 1,
    "score": 90,
    "comments": "Buen desempeño",
    "state": "Validated",
    "creationDate": "2025-01-24T20:00:00Z",
    "validatedAt": "2025-01-24T21:00:00Z",
    "totalErrors": 1,
    "totalSuccess": 3,
    "totalMeasurements": 4,
    "successRate": 0.75,
    "totalDurationMs": 30000,
    "averageErrorsPerMeasurement": 0.25,
    "measurements": []
  }
]
```
- **Nota:** Ahora incluye `evaluatedUserFullName` (nombre completo del practicante evaluado).

---

### Obtener evaluaciones por grupo y practicante
- URL: `/api/instructor/evaluations/by-group-practitioner?groupId={groupId}&userId={userId}`
- Método: `GET`
- Headers: `Authorization: Bearer {token}`
- Descripción: Devuelve evaluaciones filtradas por `groupId`, por `userId` (practicante) o por ambos. Ambos parámetros son opcionales pero se requiere al menos uno; si no se provee ninguno el endpoint responde `400 Bad Request`.

Casos de uso y ejemplos:

1) **Filtrar por ambos (grupo + practicante)**
- Request: `/api/instructor/evaluations/by-group-practitioner?groupId=10&userId=PRACTICANTE_GUID_123`
- Comportamiento: devuelve evaluaciones del grupo `10` realizadas al practicante `PRACTICANTE_GUID_123`.
- Ejemplo de response (200 OK):
```json
[
  {
    "id": 456,
    "evaluatorId": "INSTRUCTOR_GUID_1",
    "evaluatedUserId": "PRACTICANTE_GUID_123",
    "evaluatedUserFullName": "María González",
    "trunkId": 1,
    "groupId": 10,
    "evaluationConfigId": 2,
    "score": 70,
    "comments": "Evaluación...",

    "state": "Active",
    "creationDate": "2025-01-24T21:00:00Z",
    "validatedAt": null,
    "totalErrors": 3,
    "totalSuccess": 7,
    "totalMeasurements": 10,
    "successRate": 0.7,
    "measurements": []
  }
]
```

2) **Filtrar sólo por grupo**
- Request: `/api/instructor/evaluations/by-group-practitioner?groupId=10`
- Comportamiento: devuelve todas las evaluaciones que pertenecen al grupo `10` (de todos los practicantes del grupo).
- Ejemplo de response (200 OK):
```json
[
  { 
    "id": 456, 
    "evaluatedUserId": "PRACTICANTE_A",
    "evaluatedUserFullName": "Ana López",
    "groupId": 10, 
    "score": 70 
  },
  { 
    "id": 457, 
    "evaluatedUserId": "PRACTICANTE_B",
    "evaluatedUserFullName": "Luis Martín",
    "groupId": 10, 
    "score": 85 
  }
]
```

3) **Filtrar sólo por practicante (userId)**
- Request: `/api/instructor/evaluations/by-group-practitioner?userId=PRACTICANTE_GUID_123`
- Comportamiento: devuelve todas las evaluaciones del practicante `PRACTICANTE_GUID_123` (de todos los grupos donde tenga evaluaciones).
- Ejemplo de response (200 OK):
```json
[
  { 
    "id": 456, 
    "evaluatedUserId": "PRACTICANTE_GUID_123",
    "evaluatedUserFullName": "María González",
    "groupId": 10, 
    "score": 70 
  },
  { 
    "id": 498, 
    "evaluatedUserId": "PRACTICANTE_GUID_123",
    "evaluatedUserFullName": "María González",
    "groupId": 12, 
    "score": 88 
  }
]
```

4) **Sin filtros (error)**
- Request: `/api/instructor/evaluations/by-group-practitioner` (sin query params)
- Respuesta: `400 Bad Request`
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Se requiere al menos 'groupId' o 'userId' como parámetro de consulta."
}
```

---

## Configuraciones de Evaluación

### Crear configuración
- URL: `/api/instructor/evaluation-configs`
- Método: `POST`
- Headers: `Authorization: Bearer {token}`
- Body:
```json
{
  "name": "Config Avanzada",
  "maxErrors": 5,
  "maxSuccess": 40,
  "maxTime": 60,
  "maxTimeInterval": 120
}
```
- **Nota:** `maxSuccess` es **obligatorio** - número máximo de aciertos esperados.
- Respuesta: `200 OK`, configuración creada. Ejemplo:
```json
{
  "id": 1,
  "name": "Config Avanzada",
  "maxErrors": 5,
  "maxSuccess": 40,
  "maxTime": 60,
  "maxTimeInterval": 120,
  "isDefault": false,
  "creationDate": "2025-01-24T20:00:00Z"
}
```

---

### Obtener configuraciones de evaluación
- URL: `/api/instructor/evaluation-configs`
- Método: `GET`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `200 OK`. Ejemplo:
```json
[
  {
    "id": 1,
    "name": "Config Default",
    "maxErrors": 10,
    "maxSuccess": 30,
    "maxTime": 30,
    "maxTimeInterval": 60,
    "isDefault": true,
    "status": "Active",
    "creationDate": "2024-06-01T12:00:00Z"
  },
  {
    "id": 2,
    "name": "Config Avanzada",
    "maxErrors": 5,
    "maxSuccess": 40,
    "maxTime": 60,
    "maxTimeInterval": 120,
    "isDefault": false,
    "status": "Active",
    "creationDate": "2024-06-01T12:05:00Z"
  }
]
```

---

### Editar configuración
- URL: `/api/instructor/evaluation-configs/{id}`
- Método: `PUT`
- Headers: `Authorization: Bearer {token}`
- Body:
```json
{
  "name": "Config Editada",
  "maxErrors": 7,
  "maxSuccess": 35,
  "maxTime": 45,
  "maxTimeInterval": 90
}
```
- Respuesta: `200 OK`, configuración editada. Ejemplo:
```json
{
  "id": 1,
  "name": "Config Editada",
  "maxErrors": 7,
  "maxSuccess": 35,
  "maxTime": 45,
  "maxTimeInterval": 90,
  "isDefault": false,
  "status": "Active"
}
```

---

### Eliminar configuración
- URL: `/api/instructor/evaluation-configs/{id}`
- Método: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `204 No Content`.


---

### Restablecer configuración a default
- URL: `/api/instructor/groups/{groupId}/evaluation-parameters/reset`
- Método: `POST`
- Headers: `Authorization: Bearer {token}`
- Descripción: Restablece la configuración de evaluación del grupo a la configuración por defecto del sistema. Si no existe una configuración default, la crea con valores predeterminados (`maxErrors: 10`, `maxSuccess: 30`, `maxTime: 30`).
- Comportamiento:
  - Busca o crea la configuración `IsDefault = true`
  - Elimina la relación actual del grupo en `EvaluationConfigGroups`
  - Crea una nueva relación entre el grupo y la config default
- Respuesta: `200 OK` con la configuración default. Ejemplo:
```json
{
  "id": 1,
  "name": "Default",
  "maxErrors": 10,
  "maxSuccess": 30,
  "maxTime": 30,
  "isDefault": true
}
```

---

## Simulaciones (Practitioner)

### Crear simulación
- URL: `/api/practitioner/simulations/create`
- Método: `POST`
- Headers:
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`
- Body (ejemplo completo con 5 mediciones):

```json
{
  "trunkId": 1,
  "comments": "Simulación de prueba con 5 mediciones",
  "measurements": [
    {
      "timestamp": 1698150000000,
      "elapsedMs": 10000,
      "result": "OK",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "32",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    }
  ],
  "totalDurationMs": 70000,
  "totalMeasurements": 5,
  "totalSuccess": 3,
  "totalErrors": 2,
  "successRate": 0.6,
  "averageErrorsPerMeasurement": 0.4
}
```

- Respuesta esperada: `200 OK` con el objeto de la simulación creada. Ejemplo:

```json
{
  "id": 789,
  "practitionerId": "PRACTITIONER_GUID_123",
  "trunkId": 1,
  "creationDate": "2025-01-24T20:45:00Z",
  "totalDurationMs": 70000,
  "totalErrors": 2,
  "totalSuccess": 3,
  "totalMeasurements": 5,
  "successRate": 0.6,
  "averageErrorsPerMeasurement": 0.4,
  "comments": "Simulación de prueba con 5 mediciones",
  "measurements": []
}
```

---

### Obtener simulaciones del practicante
- URL: `/api/practitioner/simulations`
- Método: `GET`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `200 OK`. Ejemplo:
```json
[

  {
    "id": 1,
    "practitionerId": "PRACTICANTE_ID",
    "trunkId": 1,
    "creationDate": "2025-01-24T19:00:00Z",
    "totalDurationMs": 30000,
    "totalErrors": 1,
    "totalSuccess": 3,
    "totalMeasurements": 4,
    "successRate": 0.75,
    "averageErrorsPerMeasurement": 0.25,
    "comments": "Simulación completa"
  }
]
```

---

### Cancelar simulación
- URL: `/api/practitioner/simulations/cancel`
- Método: `POST`
- Headers: `Authorization: Bearer {token}`
- Body: mismo formato que crear simulación (completo)
- Respuesta: `200 OK`. Ejemplo:
```json
{ "message": "Simulation canceled" }
```

---

## Nuevos endpoints y cómo probarlos

### Listar practicantes (todos o por grupo)
- URL: `/api/instructor/practitioners` 
- Método: `GET`
- Params opcional: `groupId` (ej.: `/api/instructor/practitioners?groupId=123`)
- Headers: `Authorization: Bearer {token}`
- Comportamiento:
  - Sin `groupId`: devuelve todos los usuarios que tienen el rol `Practitioner`.
  - Con `groupId`: devuelve solo los practicantes que pertenecen al grupo (el grupo debe estar `Active`).
- Ejemplo (Postman):
  - Request GET `https://{HOST}/api/instructor/practitioners?groupId=5`
  - Headers: `Authorization: Bearer <TOKEN>`
  - Response ejemplo:
  ```json
  [
    { "id": "USER_ID_1", "fullname": "Nombre Uno" }, 
    { "id": "USER_ID_2", "fullname": "Nombre Dos" }
  ]
  ```

### Ranking por grupos del instructor
- URL: `/api/instructor/ranking`
- Método: `GET`
- Headers: `Authorization: Bearer {token}`
- Descripción: Devuelve un ranking (resumen por grupos) para el instructor autenticado. La estructura incluye, por grupo, indicadores agregados (p.ej. promedios de score, cantidad de evaluaciones, tasas de éxito) para ayudar a comparar desempeños entre grupos.
- Ejemplo de response (200 OK):
```json
[
  {
    "groupId": 10,
    "groupName": "Grupo A",
    "groupAverage": 78.6,
    "practitioners": [
      {
        "userId": "PRACTICANTE_GUID_1",
        "fullName": "María González",
        "averageScore": 82.5,
        "evaluationCount": 12
      },
      {
        "userId": "PRACTICANTE_GUID_2",
        "fullName": "Carlos Ruiz",
        "averageScore": 74.7,
        "evaluationCount": 8
      }
    ]
  }
]
```

### Eliminar evaluación permanentemente (sólo uso manual)
- URL: `/api/instructor/evaluations/{id}`
- Método: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Descripción: elimina físicamente la evaluación y sus mediciones de la base de datos. Uso reservado para corrección manual desde Postman o admin tools.
- Ejemplo (Postman):
  - Request DELETE `https://{HOST}/api/instructor/evaluations/123`
  - Headers: `Authorization: Bearer <TOKEN>`
  - Response: `204 No Content`

### Eliminar simulación permanentemente (sólo uso manual)
- URL: `/api/instructor/simulations/{id}`
- Método: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Descripción: elimina físicamente la simulación y sus mediciones de la base de datos. Uso reservado para corrección manual desde Postman o admin tools.
- Ejemplo (Postman):
  - Request DELETE `https://{HOST}/api/instructor/simulations/789`
  - Headers: `Authorization: Bearer <TOKEN>`
  - Response: `204 No Content`

---

### Endpoints de manejo rápido para administracción (incluye asignación/desasignación en evaluaciones)

- Asignar practicante a evaluación (POST) `https://{HOST}/api/instructor/evaluations/{evaluationId}/assign-practitioner/{userId}`
- Desasignar practicante de evaluación (POST) `https://{HOST}/api/instructor/evaluations/{evaluationId}/unassign-practitioner`

---

## Cambios recientes en la API (Enero 2025)

### 1. Campo `IsValid` eliminado de Evaluaciones
- **Qué cambió:** El campo `isValid` fue eliminado de la entidad `Evaluation` y de todos los DTOs relacionados.
- **Endpoints afectados:**
  - `POST /api/instructor/evaluations/create` - response ya no incluye `isValid`
  - `POST /api/instructor/evaluations/{id}/validate` - body ya no requiere/acepta `is_valid`
  - `GET /api/instructor/evaluations` - response ya no incluye `isValid`
  - `GET /api/instructor/evaluations/by-group-practitioner` - response ya no incluye `isValid`
- **Acción requerida:** Clientes mobile deben remover referencias a `isValid` en evaluaciones.

### 2. Campo `MaxSuccess` agregado a EvaluationConfig
- **Qué cambió:** Nuevo campo obligatorio `maxSuccess` en configuraciones de evaluación.
- **Endpoints afectados:**
  - `POST /api/instructor/evaluation-configs` - body requiere `maxSuccess`
  - `PUT /api/instructor/evaluation-configs/{id}` - body requiere `maxSuccess`
  - `GET /api/instructor/evaluation-configs` - response incluye `maxSuccess`
  - `POST /api/instructor/groups/{groupId}/evaluation-parameters/reset` - response incluye `maxSuccess`
- **Valor por defecto:** 30 aciertos
- **Acción requerida:** Clientes mobile deben incluir `maxSuccess` al crear/editar configs.

### 3. Campo `EvaluationDate` agregado a Groups
- **Qué cambió:** Nuevo campo obligatorio `evaluationDate` en grupos (fecha programada de evaluación).
- **Endpoints afectados:**
  - `POST /api/instructor/groups` - body requiere `evaluationDate`
  - `PUT /api/instructor/groups/{id}` - body requiere `evaluationDate`
  - `GET /api/instructor/groups/owned` - response incluye `evaluationDate`
- **Acción requerida:** Clientes mobile deben permitir al usuario seleccionar `evaluationDate` al crear/editar grupos.

### 4. Campo `EvaluatedUserFullName` agregado a Evaluaciones
- **Qué cambió:** Las respuestas de evaluaciones ahora incluyen el nombre completo del practicante evaluado.
- **Endpoints afectados:**
  - `GET /api/instructor/evaluaciones` - response incluye `evaluatedUserFullName`
  - `GET /api/instructor/evaluaciones/by-group-practitioner` - response incluye `evaluatedUserFullName`
  - `POST /api/instructor/evaluations/{id}/validate` - response incluye `evaluatedUserFullName`
- **Beneficio:** No es necesario hacer llamadas adicionales para obtener el nombre del practicante.

---

Última actualización: Enero 2025 - documentación actualizada con cambios de schema de base de datos.
