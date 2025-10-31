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
  "practitionerIds": ["USER_ID_1", "USER_ID_2"],
  "evaluationConfigId": 1
}
```
- Respuesta: `200 OK`, datos del grupo creado. Ejemplo:
```json
{
  "id": 123,
  "name": "Grupo A",
  "description": "Entrenamiento inicial",
  "practitionerIds": ["USER_ID_1", "USER_ID_2"],
  "evaluationConfigId": 1,
  "createdBy": "INSTRUCTOR_ID",
  "createdAt": "2025-10-24T20:00:00Z"
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
    "practitionerIds": ["USER_ID_1", "USER_ID_2"],
    "evaluationConfigId": 1
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
  "practitionerIds": ["USER_ID_1", "USER_ID_3"]
}
```
- Respuesta: `200 OK`. Ejemplo:
```json
{
  "id": 123,
  "name": "Grupo A Editado",
  "description": "Nueva descripción",
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
    "evaluationConfigId": 1,
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
  "comments": "Evaluación inicial - prueba Postman con 10 mediciones. Reglas: cada medición con al menos un status false cuenta como error; cada error resta 10 puntos del score inicial 100.",
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
    },
    {
      "timestamp": 1698140010000,
      "elapsedMs": 11000,
      "result": "OK",
      "angle_deg": "1.2",
      "angle_status": true,
      "force_value": "31",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698140020000,
      "elapsedMs": 12000,
      "result": "OUT_OF_RANGE",
      "angle_deg": "5.0",
      "angle_status": false,
      "force_value": "28",
      "force_status": true,
      "touch_status": true,
      "status": false,
      "message": "angle out of range",
      "is_valid": false
    },
    {
      "timestamp": 1698140030000,
      "elapsedMs": 13000,
      "result": "OK",
      "angle_deg": "0.5",
      "angle_status": true,
      "force_value": "30",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698140040000,
      "elapsedMs": 14000,
      "result": "OK",
      "angle_deg": "0.8",
      "angle_status": true,
      "force_value": "33",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698140050000,
      "elapsedMs": 15000,
      "result": "TOUCH_FAIL",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "29",
      "force_status": true,
      "touch_status": false,
      "status": false,
      "message": "touch sensor missed",
      "is_valid": false
    },
    {
      "timestamp": 1698140060000,
      "elapsedMs": 16000,
      "result": "OK",
      "angle_deg": "0.2",
      "angle_status": true,
      "force_value": "34",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698140070000,
      "elapsedMs": 17000,
      "result": "OK",
      "angle_deg": "0.6",
      "angle_status": true,
      "force_value": "31",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698140080000,
      "elapsedMs": 18000,
      "result": "FORCE_FAIL",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "15",
      "force_status": false,
      "touch_status": true,
      "status": false,
      "message": "force below threshold",
      "is_valid": false
    },
    {
      "timestamp": 1698140090000,
      "elapsedMs": 19000,
      "result": "OK",
      "angle_deg": "0.4",
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
- Respuesta esperada (ejemplo): `200 OK` con el objeto de la evaluación creada incluyendo agregados y la lista de mediciones. Ejemplo de response (resumen):
```json
{
  "id": 456,
  "evaluatorId": "INSTRUCTOR_GUID_1",
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "trunkId": 1,
  "groupId": 10,
  "evaluationConfigId": 2,
  "score": 70,
  "comments": "Evaluación inicial - prueba Postman con 10 mediciones...",
  "is_valid": null,
  "state": "Active",
  "totalErrors": 3,
  "totalSuccess": 7,
  "totalMeasurements": 10,
  "successRate": 0.7,
  "totalDurationMs": 145000,
  "averageErrorsPerMeasurement": 0.3,
  "measurements": [ /* objetos de medición con los mismos campos enviados y el id/time asignado */ ]
}
```

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
  "is_valid": true,
  "comments": "Validación final: cumple con los requisitos",
  "signature": "firmaInstructorEjemploBase64==",
  "evaluationConfigId": 1
}
```
- Respuesta esperada: `200 OK` con mensaje o el objeto evaluado actualizado. Ejemplo:
```json
{
  "message": "Evaluation validated",
  "evaluation": {
    "id": 456,
    "score": 92,
    "is_valid": true,
    "validatedAt": "2025-10-24T21:30:00Z",
    "signature": "firmaInstructorEjemploBase64==",
    "totalErrors": 3,
    "totalSuccess": 7,
    "totalMeasurements": 10,
    "successRate": 0.7
  }
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
- Descripción: Devuelve todas las evaluaciones creadas por el instructor autenticado. Puede incluir muchas evaluaciones de distintos grupos y practicantes.
- Ejemplo de response (mantener formato original):
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
      "timestamp": 1698140000000,
      "elapsedMs": 10000,
      "result": "OK",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "30",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    }
    // ...más mediciones
  ]
}
```

---

### Obtener evaluaciones por grupo y practicante
- URL: `/api/instructor/evaluations/by-group-practitioner?groupId={groupId}&userId={userId}`
- Método: `GET`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `200 OK`. Ejemplo (mantener formato original):
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
      "timestamp": 1698140000000,
      "elapsedMs": 10000,
      "result": "OK",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "28",
      "force_status": true,
      "touch_status": "0",
      "status": false,
      "message": "touch missing",
      "is_valid": false    
      }
    // ...más mediciones
  ]
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
  "maxTime": 60,
  "maxTimeInterval": 120
}
```
- Respuesta: `200 OK`, configuración creada. Ejemplo:
```json
{
  "id": 1,
  "name": "Config Avanzada",
  "maxErrors": 5,
  "maxTime": 60,
  "maxTimeInterval": 120,
  "isDefault": false,
  "creationDate": "2025-10-24T20:00:00Z"
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
  "maxTime": 45,
  "maxTimeInterval": 90,
  "sensorIntervals": []
}
```
- Respuesta: `200 OK`, configuración editada. Ejemplo:
```json
{
  "id": 1,
  "name": "Config Editada",
  "maxErrors": 7,
  "maxTime": 45,
  "maxTimeInterval": 90
}
```

---

### Eliminar configuración
- URL: `/api/instructor/evaluation-configs/{id}`
- Método: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `204 No Content`.

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
  "comments": "Simulación de prueba con 5 mediciones — algunos fallos incluidos para validar agregados",
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
    },
    {
      "timestamp": 1698150012000,
      "elapsedMs": 12000,
      "result": "OK",
      "angle_deg": "0.8",
      "angle_status": true,
      "force_value": "31",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698150024000,
      "elapsedMs": 14000,
      "result": "TOUCH_FAIL",
      "angle_deg": "0.5",
      "angle_status": true,
      "force_value": "29",
      "force_status": true,
      "touch_status": false,
      "status": false,
      "message": "touch missed",
      "is_valid": false
    },
    {
      "timestamp": 1698150036000,
      "elapsedMs": 16000,
      "result": "OK",
      "angle_deg": "0.3",
      "angle_status": true,
      "force_value": "33",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698150048000,
      "elapsedMs": 18000,
      "result": "FORCE_FAIL",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "12",
      "force_status": false,
      "touch_status": true,
      "status": false,
      "message": "force below threshold",
      "is_valid": false
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

- Respuesta esperada (ejemplo): `200 OK` con el objeto de la simulación creada incluyendo agregados y la lista de mediciones. Ejemplo de response (resumen):

```json
{
  "id": 789,
  "practitionerId": "PRACTITIONER_GUID_123",
  "trunkId": 1,
  "totalDurationMs": 70000,
  "totalErrors": 2,
  "totalSuccess": 3,
  "totalMeasurements": 5,
  "successRate": 0.6,
  "averageErrorsPerMeasurement": 0.4,
  "isValid": true,
  "comments": "Simulación de prueba con 5 mediciones — algunos fallos incluidos para validar agregados",
  "measurements": [
    {
      "id": 1001,
      "timestamp": 1698150000000,
      "elapsedMs": 10000,
      "result": "CORRECT",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "32",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "id": 1002,
      "timestamp": 1698150012000,
      "elapsedMs": 12000,
      "result": "CORRECT",
      "angle_deg": "0.8",
      "angle_status": true,
      "force_value": "31",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "id": 1003,
      "timestamp": 1698150024000,
      "elapsedMs": 14000,
      "result": "INCORRECT",
      "angle_deg": "0.5",
      "angle_status": true,
      "force_value": "29",
      "force_status": true,
      "touch_status": false,
      "status": false,
      "message": "touch missed",
      "is_valid": false
    },
    {
      "id": 1004,
      "timestamp": 1698150036000,
      "elapsedMs": 16000,
      "result": "CORRECT",
      "angle_deg": "0.3",
      "angle_status": true,
      "force_value": "33",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "id": 1005,
      "timestamp": 1698150048000,
      "elapsedMs": 18000,
      "result": "INCORRECT",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "12",
      "force_status": false,
      "touch_status": true,
      "status": false,
      "message": "force below threshold",
      "is_valid": false
    }
  ]
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
    "totalDurationMs": 30000,
    "totalErrors": 1,
    "totalSuccess": 3,
    "totalMeasurements": 4,
    "successRate": 0.75,
    "averageErrorsPerMeasurement": 0.25,
    "isValid": true,
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

## Nuevos endpoints y cómo probarlos (Staging y Postman)

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
  [ { "id": "USER_ID_1", "fullname": "Nombre Uno" }, { "id": "USER_ID_2", "fullname": "Nombre Dos" } ]
  ```

### Eliminar evaluación permanentemente (sólo uso manual)
- URL: `/api/instructor/evaluations/{id}`
- Método: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Descripción: elimina físicamente la evaluación y sus mediciones de la base de datos. Uso reservado para corrección manual desde Postman o admin tools.
- Ejemplo (Postman):
  - Request DELETE `https://{HOST}/api/instructor/evaluations/123`
  - Headers: `Authorization: Bearer <TOKEN>`
  - Response: `204 No Content` (si se eliminó)

### Cómo probar en Staging (ranura)
- Si usás Deployment Slots (recomendado):
  1. En el Portal de Azure -> App Services -> seleccioná tu App Service -> `Deployment slots` -> `Add Slot` -> nombre `staging`.
  2. Publicá desde Visual Studio a la ranura `staging` (en Publish -> seleccionar la ranura en lugar del sitio production).
  3. Obtené la URL de la ranura en Portal -> Deployment slots -> seleccioná `staging` -> `Overview` (ej.: `https://heimlich-api-unlam-staging.azurewebsites.net`).
  4. En Postman usá esa URL como `{HOST}` para probar: `https://heimlich-api-unlam-staging.azurewebsites.net/api/instructor/practitioners` o `.../evaluations/{id}`.
  5. Validá que los endpoints funcionen en staging (GET, DELETE, etc.).
  6. Si todo está OK, hacé `Swap` (staging ? production) desde Portal -> Deployment slots.

Notas sobre qué implica publicar a Staging vs Production
- Publicar a `staging` reinicia solo la ranura `staging`, no la ranura `production`.
- Hacer `swap` intercambia contenido y, por defecto, intercambia también las configuraciones NO marcadas como "slot setting". Marca `DefaultConnection`, `Jwt__Key`, `APPLICATIONINSIGHTS_CONNECTION_STRING` y secretos como `Slot setting` para evitar sobrescribir valores de producción.

¿Publicar desde Visual Studio directamente a Production reinicia App Service y afecta la DB?
- Sí: publicar a production reinicia la aplicación (arranque del proceso). Esto no modifica la estructura de la base de datos a menos que:
  - La aplicación ejecute migraciones automáticas en arranque (flag `ApplyMigrationsOnStartup=true` en App Service Configuration), o
  - El pipeline/publish incluya un paso explícito `dotnet ef database update`.
- En la configuración actual (por defecto) las migraciones no se ejecutan a menos que actives el flag; por tanto, publicar código no debería cambiar el schema de la DB.
- Aun así, la app se reinicia y puede haber un breve downtime (aceptado en tu horario). Recomiendo usar `staging` y `swap` para despliegues más seguros.

### Ejemplo rápido de Postman (pasos)
1. Login y obtener token:
   - POST `https://{HOST}/api/auth/login` Body JSON: `{ "userName": "tuUsuario", "password": "tuPass" }`
   - Copiar `token` de la respuesta.
2. Probar listar practicantes en staging:
   - GET `https://{STAGING_HOST}/api/instructor/practitioners?groupId=5`
   - Header: `Authorization: Bearer <TOKEN>`
3. Probar borrar evaluación (solo admin/instructor):
   - DELETE `https://{STAGING_HOST}/api/instructor/evaluations/123`
   - Header: `Authorization: Bearer <TOKEN>`

---

Contacto / pruebas
- Si el equipo mobile necesita un conjunto de ejemplos JSON para crear evaluaciones o simulaciones, en el directorio `Heimlich.Api/Examples` se incluyen varios archivos (`create-evaluation-with-aggregates.json`, `create-evaluation-no-aggregates.json`, `cancel-evaluation.json`, etc.).

---

Última actualización: (automático) documentación actualizada para pruebas desde Postman y React Native.

---

# Ejemplos Postman (nuevos)

---

### Ejemplo completo: Crear evaluación (10 mediciones, cálculo de agregados)
- URL: `/api/instructor/evaluations/create`
- Método: `POST`
- Headers:
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`
- Body (raw JSON) — este ejemplo contiene 10 mediciones. Regla de negocio aplicada en ejemplo: cada medición que tenga al menos un campo de estado `false` se considera error; cada error resta 10 puntos del score inicial (100):

```json
{
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "trunkId": 1,
  "groupId": 10,
  "comments": "Evaluación inicial - prueba Postman con 10 mediciones. Reglas: cada medición con al menos un status false cuenta como error; cada error resta 10 puntos del score inicial 100.",
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
    },
    {
      "timestamp": 1698140010000,
      "elapsedMs": 11000,
      "result": "OK",
      "angle_deg": "1.2",
      "angle_status": true,
      "force_value": "31",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698140020000,
      "elapsedMs": 12000,
      "result": "OUT_OF_RANGE",
      "angle_deg": "5.0",
      "angle_status": false,
      "force_value": "28",
      "force_status": true,
      "touch_status": true,
      "status": false,
      "message": "angle out of range",
      "is_valid": false
    },
    {
      "timestamp": 1698140030000,
      "elapsedMs": 13000,
      "result": "OK",
      "angle_deg": "0.5",
      "angle_status": true,
      "force_value": "30",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698140040000,
      "elapsedMs": 14000,
      "result": "OK",
      "angle_deg": "0.8",
      "angle_status": true,
      "force_value": "33",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698140050000,
      "elapsedMs": 15000,
      "result": "TOUCH_FAIL",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "29",
      "force_status": true,
      "touch_status": false,
      "status": false,
      "message": "touch sensor missed",
      "is_valid": false
    },
    {
      "timestamp": 1698140060000,
      "elapsedMs": 16000,
      "result": "OK",
      "angle_deg": "0.2",
      "angle_status": true,
      "force_value": "34",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698140070000,
      "elapsedMs": 17000,
      "result": "OK",
      "angle_deg": "0.6",
      "angle_status": true,
      "force_value": "31",
      "force_status": true,
      "touch_status": true,
      "status": true,
      "message": null,
      "is_valid": true
    },
    {
      "timestamp": 1698140080000,
      "elapsedMs": 18000,
      "result": "FORCE_FAIL",
      "angle_deg": "0.0",
      "angle_status": true,
      "force_value": "15",
      "force_status": false,
      "touch_status": true,
      "status": false,
      "message": "force below threshold",
      "is_valid": false
    },
    {
      "timestamp": 1698140090000,
      "elapsedMs": 19000,
      "result": "OK",
      "angle_deg": "0.4",
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

- Respuesta esperada (ejemplo): `200 OK` con el objeto de la evaluación creada incluyendo agregados y la lista de mediciones. Ejemplo de response (resumen):
```json
{
  "id": 456,
  "evaluatorId": "INSTRUCTOR_GUID_1",
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "trunkId": 1,
  "groupId": 10,
  "evaluationConfigId": 2,
  "score": 70,
  "comments": "Evaluación inicial - prueba Postman con 10 mediciones...",
  "is_valid": null,
  "state": "Active",
  "totalErrors": 3,
  "totalSuccess": 7,
  "totalMeasurements": 10,
  "successRate": 0.7,
  "totalDurationMs": 145000,
  "averageErrorsPerMeasurement": 0.3,
  "measurements": [ /* objetos de medición con los mismos campos enviados y el id/time asignado */ ]
}
```

---

### Ejemplo completo: Validar evaluación
- URL: `/api/instructor/evaluations/{evaluationId}/validate`
- Método: `POST`
- Headers:
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`
- Body (raw JSON):

```json
{
  "score": 92,
  "is_valid": true,
  "comments": "Validación final: cumple con los requisitos",
  "signature": "firmaInstructorEjemploBase64==",
  "evaluationConfigId": 1
}
```

- Respuesta esperada: `200 OK` con mensaje o el objeto evaluado actualizado. Ejemplo:

```json
{
  "message": "Evaluation validated",
  "evaluation": {
    "id": 456,
    "score": 92,
    "is_valid": true,
    "validatedAt": "2025-10-24T21:30:00Z",
    "signature": "firmaInstructorEjemploBase64==",
    "totalErrors": 3,
    "totalSuccess": 7,
    "totalMeasurements": 10,
    "successRate": 0.7
  }
}

```

---

### Eliminar simulación (solo uso manual)
- URL: `/api/instructor/simulations/{id}`
- Método: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Descripción: elimina físicamente la simulación y sus mediciones de la base de datos. Uso reservado para corrección manual desde Postman o admin tools.
- Ejemplo (Postman):
  - Request DELETE `https://{HOST}/api/instructor/simulations/789`
  - Headers: `Authorization: Bearer <TOKEN>`
  - Response: `204 No Content`.

---

(Se agregó `creationDate` a los ejemplos de response para evaluaciones y simulaciones. A continuación se muestran fragmentos actualizados con `creationDate`.)

### Ejemplo de response (evaluación con `creationDate`)
```json
{
  "id": 456,
  "evaluatorId": "INSTRUCTOR_GUID_1",
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "trunkId": 1,
  "groupId": 10,
  "evaluationConfigId": 2,
  "score": 70,
  "comments": "Evaluación inicial - prueba Postman con 10 mediciones...",
  "is_valid": null,
  "state": "Active",
  "creationDate": "2025-10-24T21:00:00Z",
  "validatedAt": null,
  "totalErrors": 3,
  "totalSuccess": 7,
  "totalMeasurements": 10,
  "successRate": 0.7,
  "totalDurationMs": 145000,
  "averageErrorsPerMeasurement": 0.3,
  "measurements": [ /* objetos de medición con los mismos campos enviados y el id/time asignado */ ]
}
```

### Ejemplo de response (simulación con `creationDate`)
```json
{
  "id": 789,
  "practitionerId": "PRACTITIONER_GUID_123",
  "trunkId": 1,
  "creationDate": "2025-10-24T20:45:00Z",
  "totalDurationMs": 70000,
  "totalErrors": 2,
  "totalSuccess": 3,
  "totalMeasurements": 5,
  "successRate": 0.6,
  "averageErrorsPerMeasurement": 0.4,
  "isValid": true,
  "comments": "Simulación de prueba con 5 mediciones — algunos fallos incluidos para validar agregados",
  "measurements": [ /* ... */ ]
}
