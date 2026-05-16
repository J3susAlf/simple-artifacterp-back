# ConfigurationSystemController Mapping

## POST /api/ConfigurationSystem/check
**Descripción:** Verifica si existe configuración del sistema y crea una por defecto si no existe.

**Request:**
- Body: vacío
- URL: sin parámetros

**Response 200:**
```json
{
  "id": "string",
  "commercialName": "string",
	"logo": "https://.../firma",
	"background": "https://.../firma",
	"costOfElectricity": 0,
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

**Errores:**
- 500: Error interno al acceder a MongoDB.

---

## GET /api/ConfigurationSystem
**Descripción:** Obtiene la configuración del sistema.

**Request:**
- Body: vacío
- URL: sin parámetros

**Response 200:**
```json
{
  "id": "string",
  "commercialName": "string",
  "logo": "string",
  "background": "string",
  "costOfElectricity": 0,
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

**Errores:**
- 404: No encontrado.
- 500: Error interno.

---

## PUT /api/ConfigurationSystem/{id}
**Descripción:** Actualiza la configuración del sistema (no permite cambiar Id ni UpdatedAt) y permite subir imagen de fondo.

**Request:**
- URL: id (obligatorio)
- Content-Type: multipart/form-data
- Body (form-data):
  - commercialName (opcional)
	- logo (opcional, string)
  - logoFile (opcional, archivo logo)
  - background (opcional, string)
  - backgroundFile (opcional, archivo imagen)
  - costOfElectricity (opcional)

**Response 200:**
```json
{
  "id": "string",
  "commercialName": "string",
  "logo": "string",
  "background": "string",
  "costOfElectricity": 0,
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

**Errores:**
- 404: No encontrado.
- 400: "Solo se permiten imágenes."
- 400: "Tipo de archivo de logo no permitido."
- 500: Error interno.

---

## POST /api/ConfigurationSystem/files
**Descripción:** Sube un archivo al bucket S3.

**Request:**
- Content-Type: multipart/form-data
- Body (form-data):
  - **file** (obligatorio): archivo

**Response 200:**
```json
{ "key": "uploads/uuid-nombre.ext" }
```

**Errores:**
- 400: "Archivo requerido."
- 500: Error interno al subir archivo.

---

## GET /api/ConfigurationSystem/files
**Descripción:** Lista archivos cargados en el bucket (prefijo uploads/).

**Request:**
- Body: vacío
- URL: sin parámetros

**Response 200:**
```json
[
  { "key": "uploads/archivo.ext", "size": 12345, "lastModified": "2024-01-01T00:00:00Z" }
]
```

**Errores:**
- 500: Error interno al listar archivos.
