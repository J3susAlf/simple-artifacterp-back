# InventoryAssortmentsController Mapping

## POST /api/inventory/assortments
**Descripcion:** Inicia un surtido. Si `immediateDelivery=true`, crea inventario y marca entregado.

**Request (JSON):**
```json
{
  "suppliesId": "string",
  "isPack": true,
  "packQuantity": 4,
  "purchaseQuantity": 2,
  "unitaryPurchaseCost": 400,
  "immediateDelivery": true,
  "deliveryDate": "2026-05-18T00:00:00Z",
  "lastEditedByName": "string"
}
```

**Response 200:**
```json
{
  "assortmentId": 1,
  "suppliesId": "string",
  "unitaryPurchaseCost": 400,
  "isPack": true,
  "packQuantity": 4,
  "purchaseQuantity": 2,
  "lastEditedByName": "string",
  "status": 2,
  "deliveryDate": "2026-05-17T12:00:00Z"
}
```

**Errores:**
- 400: "Cantidad y costo unitario deben ser mayores a cero." / "PackQuantity debe ser mayor a cero cuando es pack."
- 404: "Insumo no encontrado o id invalido."
- 500: Error interno.

---

## PUT /api/inventory/assortments/{id}/finalize
**Descripcion:** Finaliza un surtido pendiente y aplica el inventario.

**Request (JSON):**
```json
{
  "lastEditedByName": "string"
}
```

**Response 200:**
```json
{
  "assortmentId": 1,
  "suppliesId": "string",
  "unitaryPurchaseCost": 400,
  "isPack": true,
  "packQuantity": 4,
  "purchaseQuantity": 2,
  "lastEditedByName": "string",
  "status": 2,
  "deliveryDate": "2026-05-17T12:00:00Z"
}
```

**Errores:**
- 404: No encontrado.
- 500: Error interno.

---

## POST /api/inventory/assortments/direct-inventory
**Descripcion:** Alta directa de inventario de insumos.

**Request (JSON):**
```json
{
  "suppliesId": "string",
  "availableQuantity": 10,
  "minimumQuantity": 2,
  "committedQuantity": 0,
  "unitsMeasurementId": "string",
  "lastCost": 100,
  "lastEditedByName": "string"
}
```

**Response 200:**
```json
{
  "inventorySuppliesId": 1,
  "availableQuantity": 10,
  "minimumQuantity": 2,
  "committedQuantity": 0,
  "unitsMeasurementId": "string",
  "suppliesId": "string",
  "lastDispatchNumber": "DirectAssortment"
}
```

**Errores:**
- 400: "AvailableQuantity debe ser mayor a cero."
- 404: "Insumo no encontrado o id invalido."
- 500: Error interno.

---

## GET /api/inventory/assortments
**Descripcion:** Lista surtidos. Por defecto devuelve `status=Pending` e incluye datos del insumo.

**Query Params:**
- year (opcional, requiere month)
- month (opcional, 1-12, requiere year)
- status (opcional, default Pending)
- supplyType (opcional)

**Response 200:**
```json
[
  {
    "assortmentId": 1,
    "suppliesId": "string",
    "unitaryPurchaseCost": 400,
    "isPack": true,
    "packQuantity": 4,
    "purchaseQuantity": 2,
    "lastEditedByName": "string",
    "status": 1,
    "deliveryDate": "2026-05-18T00:00:00Z",
    "supply": {
      "id": "string",
      "type": 1,
      "name": "string",
      "color": 1,
      "brand": "string",
      "image": "string",
      "lastCost": 100,
      "description": "string",
      "isActive": true,
      "tax": 0,
      "unitsMeasurementId": "string"
    }
  }
]
```

**Errores:**
- 400: "Debe enviar mes y anio para filtrar por fecha." / "Mes invalido."
- 500: Error interno.

---

## GET /api/inventory/assortments/inventory-supplies
**Descripcion:** Lista inventario de insumos con datos del insumo asociado.

**Response 200:**
```json
[
  {
    "inventorySuppliesId": 1,
    "availableQuantity": 10,
    "minimumQuantity": 2,
    "committedQuantity": 0,
    "unitsMeasurementId": "string",
    "suppliesId": "string",
    "lastDispatchNumber": "DirectAssortment",
    "supply": {
      "id": "string",
      "type": 1,
      "name": "string",
      "color": 1,
      "brand": "string",
      "image": "string",
      "lastCost": 100,
      "description": "string",
      "isActive": true,
      "tax": 0,
      "unitsMeasurementId": "string"
    }
  }
]
```

**Errores:**
- 500: Error interno.
