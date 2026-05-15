# InventoryCatalogsController Mapping

## GET /api/inventory/catalogs/assets
**Descripción:** Lista activos.

**Request:**
- Body: vacío

**Response 200:**
```json
[
  { "id": "string", "name": "string", "description": "string", "price": 0, "image": "string", "electricity": 0, "wearType": 0 }
]
```

**Errores:**
- 500: Error interno.

---

## GET /api/inventory/catalogs/assets/{id}
**Descripción:** Obtiene un activo por id.

**Request:**
- URL: id (obligatorio)

**Response 200:**
```json
{ "id": "string", "name": "string", "description": "string", "price": 0, "image": "string", "electricity": 0, "wearType": 0 }
```

**Errores:**
- 404: No encontrado.
- 500: Error interno.

---

## POST /api/inventory/catalogs/assets
**Descripción:** Crea un activo (Id generado por MongoDB).

**Request:**
- Content-Type: multipart/form-data
- Body (form-data):
  - name (opcional)
  - description (opcional)
  - price (obligatorio)
  - image (opcional, string)
  - imageFile (opcional, archivo imagen)
  - electricity (obligatorio)
  - wearType (obligatorio)

**Response 200:**
```json
{ "id": "string", "name": "string", "description": "string", "price": 0, "image": "string", "electricity": 0, "wearType": 0 }
```

**Errores:**
- 400: "Solo se permiten imágenes."
- 500: Error interno.

---

## PUT /api/inventory/catalogs/assets/{id}
**Descripción:** Actualiza un activo (no permite cambiar Id).

**Request:**
- URL: id (obligatorio)
- Body (JSON):
  - name (opcional)
  - description (opcional)
  - price (obligatorio)
  - image (opcional)
  - electricity (obligatorio)
  - wearType (obligatorio)

**Response 200:**
```json
{ "id": "string", "name": "string", "description": "string", "price": 0, "image": "string", "electricity": 0, "wearType": 0 }
```

**Errores:**
- 404: No encontrado.
- 500: Error interno.

---

## DELETE /api/inventory/catalogs/assets/{id}
**Descripción:** Elimina un activo si no está en uso, o si ForceDelete=true.

**Request:**
- URL: id (obligatorio)
- Body (JSON):
  - forceDelete (opcional, default false)

**Response 200:**
- Body vacío

**Errores:**
- 404: No encontrado.
- 409: "Registro en uso."
- 500: Error interno.

---

## POST /api/inventory/catalogs/supplies/{id}/image
**Descripción:** Sube una imagen del insumo (solo imágenes) y actualiza el campo Image.

**Request:**
- URL: id (obligatorio)
- Content-Type: multipart/form-data
- Body (form-data):
  - file (obligatorio): imagen

**Response 200:**
```json
{ "id": "string", "type": 0, "name": "string", "color": 0, "brand": "string", "image": "images/supplies/name-type-yyyymmdd-guid", "lastCost": 0, "description": "string", "isActive": true, "tax": 0, "unitsMeasurementId": 0 }
```

**Errores:**
- 400: "Archivo requerido." / "Solo se permiten imágenes."
- 404: No encontrado.
- 500: Error interno.

---

## GET /api/inventory/catalogs/units-measurement
**Descripción:** Lista unidades de medida.

**Request:**
- Body: vacío

**Response 200:**
```json
[
  { "id": "string", "name": "string", "symbol": "string", "type": "string" }
]
```

**Errores:**
- 500: Error interno.

---

## GET /api/inventory/catalogs/units-measurement/{id}
**Descripción:** Obtiene unidad de medida por id.

**Request:**
- URL: id (obligatorio)

**Response 200:**
```json
{ "id": "string", "name": "string", "symbol": "string", "type": "string" }
```

**Errores:**
- 404: No encontrado.
- 500: Error interno.

---

## POST /api/inventory/catalogs/units-measurement
**Descripción:** Crea unidad de medida (Id generado por MongoDB).

**Request:**
- Body (JSON):
  - name (opcional)
  - symbol (opcional)
  - type (opcional)

**Response 200:**
```json
{ "id": "string", "name": "string", "symbol": "string", "type": "string" }
```

**Errores:**
- 500: Error interno.

---

## PUT /api/inventory/catalogs/units-measurement/{id}
**Descripción:** Actualiza unidad de medida (no permite cambiar Id).

**Request:**
- URL: id (obligatorio)
- Body (JSON):
  - name (opcional)
  - symbol (opcional)
  - type (opcional)

**Response 200:**
```json
{ "id": "string", "name": "string", "symbol": "string", "type": "string" }
```

**Errores:**
- 404: No encontrado.
- 500: Error interno.

---

## DELETE /api/inventory/catalogs/units-measurement/{id}
**Descripción:** Elimina unidad de medida si no está en uso, o si ForceDelete=true.

**Request:**
- URL: id (obligatorio)
- Body (JSON):
  - forceDelete (opcional, default false)

**Response 200:**
- Body vacío

**Errores:**
- 404: No encontrado.
- 409: "Registro en uso."
- 500: Error interno.

---

## GET /api/inventory/catalogs/supplies
**Descripción:** Lista insumos.

**Request:**
- Body: vacío

**Response 200:**
```json
[
  { "id": "string", "type": 0, "name": "string", "color": 0, "brand": "string", "image": "string", "lastCost": 0, "description": "string", "isActive": true, "tax": 0, "unitsMeasurementId": 0 }
]
```

**Errores:**
- 500: Error interno.

---

## GET /api/inventory/catalogs/supplies/{id}
**Descripción:** Obtiene un insumo por id.

**Request:**
- URL: id (obligatorio)

**Response 200:**
```json
{ "id": "string", "type": 0, "name": "string", "color": 0, "brand": "string", "image": "string", "lastCost": 0, "description": "string", "isActive": true, "tax": 0, "unitsMeasurementId": 0 }
```

**Errores:**
- 404: No encontrado.
- 500: Error interno.

---

## POST /api/inventory/catalogs/supplies
**Descripción:** Crea un insumo (Id generado por MongoDB).

**Request:**
- Content-Type: multipart/form-data
- Body (form-data):
  - type (obligatorio)
  - name (opcional)
  - color (obligatorio)
  - brand (opcional)
  - image (opcional, string)
  - imageFile (opcional, archivo imagen)
  - lastCost (opcional)
  - description (opcional)
  - isActive (obligatorio)
  - tax (opcional)
  - unitsMeasurementId (obligatorio)

**Response 200:**
```json
{ "id": "string", "type": 0, "name": "string", "color": 0, "brand": "string", "image": "string", "lastCost": 0, "description": "string", "isActive": true, "tax": 0, "unitsMeasurementId": 0 }
```

**Errores:**
- 400: "Solo se permiten imágenes."
- 500: Error interno.

---

## PUT /api/inventory/catalogs/supplies/{id}
**Descripción:** Actualiza un insumo (no permite cambiar Id).

**Request:**
- URL: id (obligatorio)
- Body (JSON):
  - type (obligatorio)
  - name (opcional)
  - color (obligatorio)
  - brand (opcional)
  - image (opcional)
  - lastCost (opcional)
  - description (opcional)
  - isActive (obligatorio)
  - tax (opcional)
  - unitsMeasurementId (obligatorio)

**Response 200:**
```json
{ "id": "string", "type": 0, "name": "string", "color": 0, "brand": "string", "image": "string", "lastCost": 0, "description": "string", "isActive": true, "tax": 0, "unitsMeasurementId": 0 }
```

**Errores:**
- 404: No encontrado.
- 500: Error interno.

---

## DELETE /api/inventory/catalogs/supplies/{id}
**Descripción:** Elimina un insumo si no está en uso, o si ForceDelete=true.

**Request:**
- URL: id (obligatorio)
- Body (JSON):
  - forceDelete (opcional, default false)

**Response 200:**
- Body vacío

**Errores:**
- 404: No encontrado.
- 409: "Registro en uso."
- 500: Error interno.
