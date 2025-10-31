# Heimlich API - Documentaci�n de Endpoints

Base URL (producci�n)
- https://heimlich-api-unlam.azurewebsites.net

Encabezados comunes
- `Content-Type: application/json`
- `Authorization: Bearer {token}` (para endpoints protegidos)

Token JWT
- El endpoint de login devuelve un JWT en el cuerpo. El campo puede llamarse `token` o `access_token` dependiendo de la versi�n del cliente. Usa ese valor en el header `Authorization: Bearer {token}`.
- Nota de configuraci�n: la clave JWT (`Jwt__Key`) y valores `Jwt__Issuer` / `Jwt__Audience` se configuran en App Service -> Configuration (o en Key Vault) en producci�n. Aseg�rate de que la app m�vil use el mismo `Audience` que est� configurado en la API.

C�mo probar r�pidamente
- Postman: POST al endpoint de login, copiar token y usar en Authorization -> Bearer Token.
- curl ejemplo:
  - Obtener token:
    `curl -X POST -H "Content-Type: application/json" -d '{"userName":"tuUsuario","password":"tuPass"}' https://heimlich-api-unlam.azurewebsites.net/api/auth/login`
  - Llamar endpoint protegido:
    `curl -H "Authorization: Bearer <TOKEN>" https://heimlich-api-unlam.azurewebsites.net/api/instructor/evaluations`
- React Native (fetch + AsyncStorage):
  - Guardar token tras login y a�adir `Authorization` en las fetch posteriores. (Ver ejemplos al final del documento.)

Nota sobre CORS
- Aplicaciones nativas (React Native Android/iOS) no requieren configuraci�n CORS.
- Aplicaciones web/SPA deben tener su origen a�adido en la pol�tica CORS del backend. En producci�n el origen esperado es `https://heimlich-app-mobile.azurestaticapps.net`.
- Si necesitas otro origen, pedir que lo agreguen en App Service -> Configuration o modificar la pol�tica CORS en `Program.cs`.

Swagger
- Swagger est� deshabilitado por defecto en producci�n (`EnableSwagger=false`). Para ver la documentaci�n en desarrollo activa `EnableSwagger=true` en configuraci�n o revisa localmente con `dotnet run`.

Application Insights
- Application Insights est� habilitado en producci�n y la cadena de conexi�n se encuentra en App Service -> Configuration como `APPLICATIONINSIGHTS_CONNECTION_STRING`.
- Puedes usar `Live Metrics` para ver peticiones en tiempo real y `Failures` para revisar excepciones.

Autenticaci�n
- Todos los endpoints requieren JWT salvo `/api/auth/register` y `/api/auth/login`.
- Aseg�rate de usar el `Issuer` y `Audience` configurados en la app para validar tokens.

Notas sobre despliegue y base de datos
- En producci�n la aplicaci�n usa la cadena `DefaultConnection` configurada en App Service -> Connection strings. En este proyecto hemos configurado la aplicaci�n para soportar autenticaci�n mediante Managed Identity (Azure AD) usando `Authentication=Active Directory Default` en la cadena de conexi�n.
- Para que la API pueda crear las tablas mediante las migraciones al arrancar, la Managed Identity del App Service debe existir y tener permisos en la base de datos (p.ej. `db_owner` temporal o roles `db_ddladmin`/`db_datareader`/`db_datawriter`).
- Por seguridad, la ejecuci�n autom�tica de migraciones est� deshabilitada por defecto; puede activarse temporalmente con `ApplyMigrationsOnStartup=true` en App Service -> Configuration.

---

# Endpoints

## Autenticaci�n

### Registrar usuario
- URL: `/api/auth/register`
- M�todo: `POST`
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
- M�todo: `POST`
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
- M�todo: `POST`
- Headers: `Authorization: Bearer {token}`
- Body: vac�o
- Respuesta: `200 OK`. Ejemplo:
```json
{ "message": "Logout successful" }
```

---

### Perfil de usuario
- URL: `/api/auth/profile?userId={id}`
- M�todo: `GET`
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

### Cambiar contrase�a
- URL: `/api/auth/change-password`
- M�todo: `POST`
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
- M�todo: `POST`
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
- M�todo: `GET`
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
- M�todo: `PUT`
- Headers: `Authorization: Bearer {token}`
- Body:
```json
{
  "name": "Grupo A Editado",
  "description": "Nueva descripci�n",
  "practitionerIds": ["USER_ID_1", "USER_ID_3"]
}
```
- Respuesta: `200 OK`. Ejemplo:
```json
{
  "id": 123,
  "name": "Grupo A Editado",
  "description": "Nueva descripci�n",
  "practitionerIds": ["USER_ID_1", "USER_ID_3"]
}
```

---

### Eliminar grupo
- URL: `/api/instructor/groups/{id}`
- M�todo: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `204 No Content`.

---

### Asignar configuraci�n de evaluaci�n a grupo
- URL: `/api/instructor/groups/{groupId}/config/{configId}`
- M�todo: `POST`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `200 OK`, configuraci�n vinculada. Ejemplo:
```json
{ "message": "Configuration assigned" }
```

---

### Visualizar grupos asignados a practicante
- URL: `/api/instructor/groups/assigned?userId={userId}`
- M�todo: `GET`
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

### Crear evaluaci�n
- URL: `/api/instructor/evaluations/create`
- M�todo: `POST`
- Headers:
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`
- Body (completo, ejemplo con 10 mediciones y agregados):
```json
{
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "trunkId": 1,
  "groupId": 10,
  "comments": "Evaluaci�n inicial - prueba Postman con 10 mediciones. Reglas: cada medici�n con al menos un status false cuenta como error; cada error resta 10 puntos del score inicial 100.",
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
- Respuesta esperada (ejemplo): `200 OK` con el objeto de la evaluaci�n creada incluyendo agregados y la lista de mediciones. Ejemplo de response (resumen):
```json
{
  "id": 456,
  "evaluatorId": "INSTRUCTOR_GUID_1",
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "trunkId": 1,
  "groupId": 10,
  "evaluationConfigId": 2,
  "score": 70,
  "comments": "Evaluaci�n inicial - prueba Postman con 10 mediciones...",
  "is_valid": null,
  "state": "Active",
  "totalErrors": 3,
  "totalSuccess": 7,
  "totalMeasurements": 10,
  "successRate": 0.7,
  "totalDurationMs": 145000,
  "averageErrorsPerMeasurement": 0.3,
  "measurements": [ /* objetos de medici�n con los mismos campos enviados y el id/time asignado */ ]
}
```

---

### Validar evaluaci�n
- URL: `/api/instructor/evaluations/{evaluationId}/validate`
- M�todo: `POST`
- Headers:
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`
- Body (raw JSON):
```json
{
  "score": 92,
  "is_valid": true,
  "comments": "Validaci�n final: cumple con los requisitos",
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

### Cancelar evaluaci�n
- URL: `/api/instructor/evaluations/cancel`
- M�todo: `POST`
- Headers: `Authorization: Bearer {token}`
- Body: mismo formato que crear evaluaci�n (completo)
- Respuesta: `200 OK`, evaluaci�n cancelada. Ejemplo:
```json
{ "message": "Evaluation canceled" }
```

---

### Obtener todas las evaluaciones del instructor
- URL: `/api/instructor/evaluations`
- M�todo: `GET`
- Headers: `Authorization: Bearer {token}`
- Descripci�n: Devuelve todas las evaluaciones creadas por el instructor autenticado. Puede incluir muchas evaluaciones de distintos grupos y practicantes.
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
  "comments": "Buen desempe�o",
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
    // ...m�s mediciones
  ]
}
```

---

### Obtener evaluaciones por grupo y practicante
- URL: `/api/instructor/evaluations/by-group-practitioner?groupId={groupId}&userId={userId}`
- M�todo: `GET`
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
    // ...m�s mediciones
  ]
}
```
---

## Configuraciones de Evaluaci�n

### Crear configuraci�n
- URL: `/api/instructor/evaluation-configs`
- M�todo: `POST`
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
- Respuesta: `200 OK`, configuraci�n creada. Ejemplo:
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

### Obtener configuraciones de evaluaci�n
- URL: `/api/instructor/evaluation-configs`
- M�todo: `GET`
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

### Editar configuraci�n
- URL: `/api/instructor/evaluation-configs/{id}`
- M�todo: `PUT`
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
- Respuesta: `200 OK`, configuraci�n editada. Ejemplo:
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

### Eliminar configuraci�n
- URL: `/api/instructor/evaluation-configs/{id}`
- M�todo: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Respuesta: `204 No Content`.

---

## Simulaciones (Practitioner)

### Crear simulaci�n
- URL: `/api/practitioner/simulations/create`
- M�todo: `POST`
- Headers:
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`
- Body (ejemplo completo con 5 mediciones):

```json
{
  "trunkId": 1,
  "comments": "Simulaci�n de prueba con 5 mediciones � algunos fallos incluidos para validar agregados",
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

- Respuesta esperada (ejemplo): `200 OK` con el objeto de la simulaci�n creada incluyendo agregados y la lista de mediciones. Ejemplo de response (resumen):

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
  "comments": "Simulaci�n de prueba con 5 mediciones � algunos fallos incluidos para validar agregados",
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
- M�todo: `GET`
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
    "comments": "Simulaci�n completa"
  }
]
```

---

### Cancelar simulaci�n
- URL: `/api/practitioner/simulations/cancel`
- M�todo: `POST`
- Headers: `Authorization: Bearer {token}`
- Body: mismo formato que crear simulaci�n (completo)
- Respuesta: `200 OK`. Ejemplo:
```json
{ "message": "Simulation canceled" }
```

---

## Nuevos endpoints y c�mo probarlos (Staging y Postman)

### Listar practicantes (todos o por grupo)
- URL: `/api/instructor/practitioners` 
- M�todo: `GET`
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

### Eliminar evaluaci�n permanentemente (s�lo uso manual)
- URL: `/api/instructor/evaluations/{id}`
- M�todo: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Descripci�n: elimina f�sicamente la evaluaci�n y sus mediciones de la base de datos. Uso reservado para correcci�n manual desde Postman o admin tools.
- Ejemplo (Postman):
  - Request DELETE `https://{HOST}/api/instructor/evaluations/123`
  - Headers: `Authorization: Bearer <TOKEN>`
  - Response: `204 No Content` (si se elimin�)

### C�mo probar en Staging (ranura)
- Si us�s Deployment Slots (recomendado):
  1. En el Portal de Azure -> App Services -> seleccion� tu App Service -> `Deployment slots` -> `Add Slot` -> nombre `staging`.
  2. Public� desde Visual Studio a la ranura `staging` (en Publish -> seleccionar la ranura en lugar del sitio production).
  3. Obten� la URL de la ranura en Portal -> Deployment slots -> seleccion� `staging` -> `Overview` (ej.: `https://heimlich-api-unlam-staging.azurewebsites.net`).
  4. En Postman us� esa URL como `{HOST}` para probar: `https://heimlich-api-unlam-staging.azurewebsites.net/api/instructor/practitioners` o `.../evaluations/{id}`.
  5. Valid� que los endpoints funcionen en staging (GET, DELETE, etc.).
  6. Si todo est� OK, hac� `Swap` (staging ? production) desde Portal -> Deployment slots.

Notas sobre qu� implica publicar a Staging vs Production
- Publicar a `staging` reinicia solo la ranura `staging`, no la ranura `production`.
- Hacer `swap` intercambia contenido y, por defecto, intercambia tambi�n las configuraciones NO marcadas como "slot setting". Marca `DefaultConnection`, `Jwt__Key`, `APPLICATIONINSIGHTS_CONNECTION_STRING` y secretos como `Slot setting` para evitar sobrescribir valores de producci�n.

�Publicar desde Visual Studio directamente a Production reinicia App Service y afecta la DB?
- S�: publicar a production reinicia la aplicaci�n (arranque del proceso). Esto no modifica la estructura de la base de datos a menos que:
  - La aplicaci�n ejecute migraciones autom�ticas en arranque (flag `ApplyMigrationsOnStartup=true` en App Service Configuration), o
  - El pipeline/publish incluya un paso expl�cito `dotnet ef database update`.
- En la configuraci�n actual (por defecto) las migraciones no se ejecutan a menos que actives el flag; por tanto, publicar c�digo no deber�a cambiar el schema de la DB.
- Aun as�, la app se reinicia y puede haber un breve downtime (aceptado en tu horario). Recomiendo usar `staging` y `swap` para despliegues m�s seguros.

### Ejemplo r�pido de Postman (pasos)
1. Login y obtener token:
   - POST `https://{HOST}/api/auth/login` Body JSON: `{ "userName": "tuUsuario", "password": "tuPass" }`
   - Copiar `token` de la respuesta.
2. Probar listar practicantes en staging:
   - GET `https://{STAGING_HOST}/api/instructor/practitioners?groupId=5`
   - Header: `Authorization: Bearer <TOKEN>`
3. Probar borrar evaluaci�n (solo admin/instructor):
   - DELETE `https://{STAGING_HOST}/api/instructor/evaluations/123`
   - Header: `Authorization: Bearer <TOKEN>`

---

Contacto / pruebas
- Si el equipo mobile necesita un conjunto de ejemplos JSON para crear evaluaciones o simulaciones, en el directorio `Heimlich.Api/Examples` se incluyen varios archivos (`create-evaluation-with-aggregates.json`, `create-evaluation-no-aggregates.json`, `cancel-evaluation.json`, etc.).

---

�ltima actualizaci�n: (autom�tico) documentaci�n actualizada para pruebas desde Postman y React Native.

---

# Ejemplos Postman (nuevos)

---

### Ejemplo completo: Crear evaluaci�n (10 mediciones, c�lculo de agregados)
- URL: `/api/instructor/evaluations/create`
- M�todo: `POST`
- Headers:
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`
- Body (raw JSON) � este ejemplo contiene 10 mediciones. Regla de negocio aplicada en ejemplo: cada medici�n que tenga al menos un campo de estado `false` se considera error; cada error resta 10 puntos del score inicial (100):

```json
{
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "trunkId": 1,
  "groupId": 10,
  "comments": "Evaluaci�n inicial - prueba Postman con 10 mediciones. Reglas: cada medici�n con al menos un status false cuenta como error; cada error resta 10 puntos del score inicial 100.",
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

- Respuesta esperada (ejemplo): `200 OK` con el objeto de la evaluaci�n creada incluyendo agregados y la lista de mediciones. Ejemplo de response (resumen):
```json
{
  "id": 456,
  "evaluatorId": "INSTRUCTOR_GUID_1",
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "trunkId": 1,
  "groupId": 10,
  "evaluationConfigId": 2,
  "score": 70,
  "comments": "Evaluaci�n inicial - prueba Postman con 10 mediciones...",
  "is_valid": null,
  "state": "Active",
  "totalErrors": 3,
  "totalSuccess": 7,
  "totalMeasurements": 10,
  "successRate": 0.7,
  "totalDurationMs": 145000,
  "averageErrorsPerMeasurement": 0.3,
  "measurements": [ /* objetos de medici�n con los mismos campos enviados y el id/time asignado */ ]
}
```

---

### Ejemplo completo: Validar evaluaci�n
- URL: `/api/instructor/evaluations/{evaluationId}/validate`
- M�todo: `POST`
- Headers:
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`
- Body (raw JSON):

```json
{
  "score": 92,
  "is_valid": true,
  "comments": "Validaci�n final: cumple con los requisitos",
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

### Eliminar simulaci�n (solo uso manual)
- URL: `/api/instructor/simulations/{id}`
- M�todo: `DELETE`
- Headers: `Authorization: Bearer {token}`
- Descripci�n: elimina f�sicamente la simulaci�n y sus mediciones de la base de datos. Uso reservado para correcci�n manual desde Postman o admin tools.
- Ejemplo (Postman):
  - Request DELETE `https://{HOST}/api/instructor/simulations/789`
  - Headers: `Authorization: Bearer <TOKEN>`
  - Response: `204 No Content`.

---

(Se agreg� `creationDate` a los ejemplos de response para evaluaciones y simulaciones. A continuaci�n se muestran fragmentos actualizados con `creationDate`.)

### Ejemplo de response (evaluaci�n con `creationDate`)
```json
{
  "id": 456,
  "evaluatorId": "INSTRUCTOR_GUID_1",
  "evaluatedUserId": "PRACTICANTE_GUID_123",
  "trunkId": 1,
  "groupId": 10,
  "evaluationConfigId": 2,
  "score": 70,
  "comments": "Evaluaci�n inicial - prueba Postman con 10 mediciones...",
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
  "measurements": [ /* objetos de medici�n con los mismos campos enviados y el id/time asignado */ ]
}
```

### Ejemplo de response (simulaci�n con `creationDate`)
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
  "comments": "Simulaci�n de prueba con 5 mediciones � algunos fallos incluidos para validar agregados",
  "measurements": [ /* ... */ ]
}
