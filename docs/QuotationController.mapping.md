# QuotationController Mapping

## GET /api/quotations
**Descripcion:** Lista cotizaciones con los campos minimos para la vista de lista y permite filtrar.

**Query Params:**
- year (opcional, requiere month)
- month (opcional, 1-12, requiere year)
- productType (opcional)
- status (opcional)
- search (opcional, busca por titulo con contains y no requiere coincidencia exacta)

**Response 200:**
```json
[
	{
		"quotationId": 1,
		"title": "string",
		"date": "2026-05-21T00:00:00Z",
		"status": 1,
		"productType": 0,
		"images": ["https://..."],
		"lastEditedByName": "string"
	}
]
```

**Errores:**
- 400: "Debe enviar mes y anio para filtrar por fecha." / "Mes invalido."
- 500: Error interno.

---

## GET /api/quotations/context
**Descripcion:** Carga datos base para crear o editar. Siempre devuelve catalogos, enums y costo de electricidad. Si `quotationId` viene, incluye el detalle de la cotizacion.

**Query Params:**
- quotationId (opcional)

**Response 200:**
```json
{
	"quotation": {
		"quotation": {
			"quotationId": 1,
			"title": "string",
			"date": "2026-05-21T00:00:00Z",
			"clientId": 10,
			"description": "string",
			"status": 1,
			"productType": 0,
			"images": ["https://..."],
			"lastEditedByName": "string"
		},
		"version": {
			"quotationVersionId": 1,
			"quotationId": 1,
			"subDescription": "string",
			"versionNumber": 1,
			"profitMargin": 20,
			"profit": 1200,
			"extraCosts": [{ "type": "shipping", "cost": 50 }],
			"productTax": 0,
			"laborCost": 200,
			"lastEditedByName": "string",
			"discount": 0,
			"subTotal": 6000,
			"totalCost": 6000
		},
		"supplies": [
			{
				"suppliesQuotationId": 1,
				"suppliesId": "string",
				"usageQuantity": 2,
				"cost": 100,
				"subTotal": 100,
				"lastEditedByName": "string",
				"supply": { "id": "string", "name": "string" }
			}
		],
		"assets": [
			{
				"assetsQuotationId": 1,
				"assetsId": "string",
				"usageQuantity": 3,
				"cost": 50,
				"subTotal": 50,
				"asset": { "id": "string", "name": "string" }
			}
		],
		"costBreakdown": {
			"materialsCost": 100,
			"assetsCost": 50,
			"laborCost": 200,
			"extraCosts": 50,
			"baseCost": 400,
			"supplies": [
				{
					"suppliesId": "string",
					"name": "string",
					"usageQuantity": 2,
					"unitCost": 50,
					"cost": 100,
					"subTotal": 100
				}
			],
			"profitMargin": 20,
			"profit": 80,
			"subTotal": 480,
			"discount": 0,
			"productTax": 0,
			"totalCost": 480
		}
	},
	"assetsCatalog": [
		{ "id": "string", "name": "string", "description": "string", "price": 0, "image": "string", "electricity": 0, "wearType": 0 }
	],
	"suppliesCatalog": [
		{ "id": "string", "type": 0, "name": "string", "color": 0, "brand": "string", "image": "string", "lastCost": 0, "costQuantity": 1000, "unitCost": 0.5, "description": "string", "isActive": true, "tax": 0, "unitsMeasurementId": "string" }
	],
	"productTypes": [{ "value": 0, "name": "Default" }],
	"quoteStatuses": [{ "value": 0, "name": "draft" }],
	"costOfElectricity": 0
}
```

**Errores:**
- 404: No encontrado.
- 500: Error interno.

---

## POST /api/quotations
**Descripcion:** Crea una cotizacion. Sube imagenes en el mismo endpoint y calcula costos.

**Request:**
- Content-Type: multipart/form-data
- Body (form-data):
	- payloadJson (obligatorio): JSON string
	- imageFiles (opcional): archivos imagen

**payloadJson ejemplo:**
```json
{
	"title": "string",
	"date": "2026-05-21T00:00:00Z",
	"clientId": 10,
	"description": "string",
	"status": 1,
	"productType": 0,
	"images": ["images/quotations/old.png"],
	"lastEditedByName": "string",
	"version": {
		"subDescription": "string",
		"versionNumber": 1,
		"profitMargin": 20,
		"extraCosts": [{ "type": "shipping", "cost": 50 }],
		"productTax": 0,
		"laborCost": 200,
		"discount": 0,
		"lastEditedByName": "string"
	},
	"supplies": [{ "suppliesId": "string", "usageQuantity": 2 }],
	"assets": [{ "assetsId": "string", "usageQuantity": 3 }]
}
```

**Response 200:**
- Mismo formato que `GET /api/quotations/context`.

**Notas de negocio:**
- `version.versionNumber` por defecto es 1 si no se envia.
- `profit`, `subTotal` y `totalCost` se calculan en el servidor.
- `extraCosts` es una lista de objetos con `type` y `cost`.
- `images` puede incluir claves existentes y `imageFiles` agrega nuevas.

**Errores:**
- 400: "PayloadJson requerido." / "PayloadJson invalido." / "Solo se permiten imagenes." / validaciones.
- 500: Error interno.

---

## PUT /api/quotations/{id}
**Descripcion:** Edita una cotizacion y recalcula precios.

**Request:**
- URL: id (obligatorio)
- Content-Type: multipart/form-data
- Body (form-data):
	- payloadJson (obligatorio): JSON string
	- imageFiles (opcional): archivos imagen

**Response 200:**
- Mismo formato que `GET /api/quotations/context`.

**Notas de negocio:**
- Recalcula costos con los datos enviados.
- Devuelve el mismo modelo de respuesta que el flujo de creacion.

**Errores:**
- 400: "PayloadJson requerido." / "PayloadJson invalido." / "Solo se permiten imagenes." / validaciones.
- 404: No encontrado.
- 500: Error interno.

---

## GET /api/quotations/catalogs/supplies
**Descripcion:** Busca insumos por nombre usando contains. Si no se envia, devuelve todos.

**Query Params:**
- name (opcional)

**Response 200:**
```json
[
	{ "id": "string", "type": 0, "name": "string", "color": 0, "brand": "string", "image": "string", "lastCost": 0, "costQuantity": 1000, "unitCost": 0.5, "description": "string", "isActive": true, "tax": 0, "unitsMeasurementId": "string" }
]
```

**Errores:**
- 500: Error interno.

---

## Logica de calculo de cotizacion
**Resumen:** El calculo se realiza en el servidor usando los insumos, activos, mano de obra, costos extra, margen de ganancia y descuento.

**Datos de entrada usados:**
- `supplies`: cada item usa `usageQuantity` y el costo unitario del insumo. El costo unitario se calcula en el servidor con `lastCost / costQuantity`.
- `assets`: cada item usa `usageQuantity`, `electricity` del activo y `costOfElectricity` de `SystemConfiguration`.
- `version.laborCost`: costo de mano de obra.
- `version.extraCosts`: lista de objetos `{ type, cost }`.
- `version.profitMargin`: porcentaje de ganancia sobre el costo base.
- `version.discount`: descuento final.
- `version.productTax`: impuesto adicional (opcional, puede ser 0).

**Formulas:**
- `unitCost = supply.lastCost / supply.costQuantity`
- `suppliesCost = sum(usageQuantity * unitCost)`
- `assetsCost = sum(usageQuantity * asset.electricity * costOfElectricity)`
- `extraCostsTotal = sum(extraCosts.cost)`
- `baseCost = suppliesCost + assetsCost + laborCost + extraCostsTotal`
- `profit = baseCost * (profitMargin / 100)`
- `subTotal = baseCost + profit`
- `totalCost = subTotal - discount + productTax`

**Notas:**
- `profit`, `subTotal` y `totalCost` se calculan siempre en el servidor.
- Si faltan insumos o activos validos, la cotizacion no se procesa.

## Desglose de costos de la cotizacion
**Resumen:** Se muestra como se compone el costo final a partir de materiales, activos, mano de obra, extras y margen.

**Componentes:**
- **Materiales (suppliesCost):** suma del costo de insumos usados.
- **Activos o maquinas (assetsCost):** costo por uso de activos con base en electricidad.
- **Mano de obra (laborCost):** costo directo de trabajo.
- **Costos extra (extraCostsTotal):** suma de extras.
- **Base de calculo (baseCost):** suma de materiales + activos + mano de obra + extras.
- **Margen (profit):** porcentaje aplicado a la base.
- **SubTotal:** base + margen.
- **Descuento (discount):** se resta al subtotal.
- **Impuesto (productTax):** se suma al final.
- **Total:** subtotal - descuento + impuesto.

**Formulas resumidas:**
- `materials = suppliesCost`
- `machines = assetsCost`
- `extras = extraCostsTotal`
- `baseCost = materials + machines + laborCost + extras`
- `profit = baseCost * (profitMargin / 100)`
- `subTotal = baseCost + profit`
- `total = subTotal - discount + productTax`

---

## GET /api/quotations/catalogs/assets
**Descripcion:** Busca activos por nombre usando contains. Si no se envia, devuelve todos.

**Query Params:**
- name (opcional)

**Response 200:**
```json
[
	{ "id": "string", "name": "string", "description": "string", "price": 0, "image": "string", "electricity": 0, "wearType": 0 }
]
```

**Errores:**
- 500: Error interno.
