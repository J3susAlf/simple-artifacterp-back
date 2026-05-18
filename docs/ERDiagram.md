---
id: entity-relationship-diagram
title: Diagrama Entidad-Relación - Simple ArtifactERP
---

erDiagram
    USER ||--o{ USER_DEVICE : "tiene"
    USER ||--o{ QUOTATION : "crea"
    CLIENT ||--o{ SALE : "realiza"
    SALE ||--|| QUOTATION : "basada en"
    SALE ||--o{ PRODUCT : "contiene"
    QUOTATION ||--o{ QUOTATION_VERSION : "tiene"
    QUOTATION_VERSION ||--o{ ASSETS_QUOTATION : "incluye"
    QUOTATION_VERSION ||--o{ SUPPLIES_QUOTATION : "incluye"
    ASSETS ||--o{ ASSETS_QUOTATION : "cotizado en"
    SUPPLIES ||--o{ SUPPLIES_QUOTATION : "cotizado en"
    SUPPLIES ||--o{ ASSORTMENT : "comprado en"
    SUPPLIES ||--o{ INVENTORY_SUPPLIES : "registra"
    SUPPLIES ||--|| UNITS_MEASUREMENT : "usa"
    PRODUCT ||--o{ INVENTORY_PRODUCT : "registra"
    SYSTEM_CONFIGURATION ||--|| SYSTEM_CONFIGURATION : "única"

    USER {
        string id PK
        string email
        string userName
        string displayName
        string passwordHash
        string googleId
        string profilePhotoUrl
        enum userType
        enum role
        boolean isActive
        datetime createdAt
        datetime lastLoginAt
    }

    USER_DEVICE {
        int userDeviceId PK
        int userId FK
        string deviceId
        string platform
        string deviceName
        string pushToken
        datetime lastSeenAt
        boolean isActiveSession
    }

    CLIENT {
        int clientId PK
        string nickname
        string firstName
        string lastName
        string address
        string type
    }

    SALE {
        int saleId PK
        int clientId FK
        int quotationId FK
        string paymentsJson
        string description
        string saleType
        enum status
        string locationJson
        datetime date
        decimal totalCost
        string lastEditedByName
    }

    PRODUCT {
        int productId PK
        int saleId FK
        string name
        string description
        string images
        decimal quantity
        string status
        datetime date
    }

    QUOTATION {
        int quotationId PK
        int clientId FK
        string title
        datetime date
        string description
        string status
        enum productType
        string images
        string lastEditedByName
    }

    QUOTATION_VERSION {
        int quotationVersionId PK
        int quotationId FK
        string description
        int versionNumber
        decimal profitMargin
        decimal profit
        string extraCostsJson
        decimal productTax
        decimal laborCost
        string lastEditedByName
    }

    ASSETS_QUOTATION {
        int assetsQuotationId PK
        int assetsId FK
        int quotationVersionId FK
        decimal usageQuantity
        decimal cost
        decimal subTotal
    }

    ASSETS {
        string id PK
        string name
        string description
        decimal price
        string image
        decimal electricity
        enum wearType
    }

    SUPPLIES_QUOTATION {
        int suppliesQuotationId PK
        int suppliesId FK
        int quotationVersionId FK
        decimal usageQuantity
        decimal cost
        decimal subTotal
        string lastEditedByName
    }

    SUPPLIES {
        string id PK
        enum type
        string name
        enum color
        string brand
        string image
        decimal lastCost
        string description
        boolean isActive
        decimal tax
        string unitsMeasurementId FK
    }

    UNITS_MEASUREMENT {
        string id PK
        string name
        string symbol
        string type
    }

    ASSORTMENT {
        int assortmentId PK
        int suppliesId FK
        decimal totalPurchaseCost
        int purchaseQuantity
        string lastEditedByName
    }

    INVENTORY_SUPPLIES {
        int inventorySuppliesId PK
        int suppliesId FK
        decimal availableQuantity
        decimal minimumQuantity
        decimal committedQuantity
        decimal lastDispatchNumber
    }

    INVENTORY_PRODUCT {
        int inventoryProductId PK
        int productId FK
        decimal availableQuantity
        decimal committedQuantity
        decimal minimumQuantity
        datetime date
    }

    SYSTEM_CONFIGURATION {
        string id PK
        string commercialName
        string logo
        string background
        double costOfElectricity
        datetime updatedAt
    }
