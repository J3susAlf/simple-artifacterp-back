# SystemManagerController Mapping

## POST /api/SystemManager/register

**Descripción:** Registra un usuario nuevo.

**Request:**

- Body (JSON):
  - userName (opcional)
  - email (obligatorio)
  - password (obligatorio)
  - displayName (opcional)
  - userType (obligatorio)
  - role (obligatorio)

**Response 200:**

```json
{ "id": "string", "email": "string", "userName": "string" }
```

**Errores:**

- 400: "Email y contraseña requeridos."
- 409: "El correo ya está registrado."
- 500: Error interno.

---

## POST /api/SystemManager/login

**Descripción:** Inicia sesión y devuelve un token JWT.

**Request:**

- Body (JSON):
  - userName (obligatorio)
  - password (obligatorio)

**Response 200:**

```json
{
  "token": "string",
  "userName": "string",
  "email": "string",
  "userType": 0,
  "role": 0
}
```

**Errores:**

- 400: "Usuario y contraseña requeridos."
- 401: "Credenciales inválidas."
- 500: Error interno.

---

## GET /api/SystemManager/users

**Descripción:** Lista usuarios existentes.

**Request:**

- Body: vacío

**Response 200:**

```json
[
  {
    "id": "string",
    "userName": "string",
    "email": "string",
    "displayName": "string",
    "userType": 0,
    "role": 0
  }
]
```

**Errores:**

- 500: Error interno.

---

## PUT /api/SystemManager/profile/{id}

**Descripción:** Edita el perfil del usuario (sin modificar Id).

**Request:**

- URL: id (obligatorio)
- Body (JSON):
  - displayName (opcional)
  - email (opcional)

**Response 200:**

```json
{ "id": "string", "displayName": "string", "email": "string" }
```

**Errores:**

- 404: No encontrado.
- 500: Error interno.

---

## PUT /api/SystemManager/password/{id}

**Descripción:** Cambia la contraseña del usuario.

**Request:**

- URL: id (obligatorio)
- Body (JSON):
  - currentPassword (obligatorio)
  - newPassword (obligatorio)

**Response 200:**

- Body vacío

**Errores:**

- 400: "Contraseña actual y nueva son requeridas."
- 401: "Credenciales inválidas."
- 500: Error interno.

---

## POST /api/SystemManager/profile/{id}/photo

**Descripción:** Sube foto de perfil al bucket.

**Request:**

- URL: id (obligatorio)
- Content-Type: multipart/form-data
- Body (form-data):
  - **file** (obligatorio): archivo

**Response 200:**

```json
{ "id": "string", "profilePhotoUrl": "profiles/uuid-archivo.ext" }
```

**Errores:**

- 400: "Archivo requerido."
- 404: No encontrado.
- 500: Error interno.
