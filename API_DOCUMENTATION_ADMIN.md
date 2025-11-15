# API Документация - Административные эндпоинты

## 📋 Содержание
1. [Аутентификация](#аутентификация)
2. [Аналитика и отчеты](#аналитика-и-отчеты)
3. [Dashboard](#dashboard)
4. [Экспорт данных](#экспорт-данных)
5. [Коды ошибок](#коды-ошибок)
6. [Примеры для Kotlin/Android](#примеры-для-kotlinandroid)

---

## Аутентификация

**Все административные endpoint'ы требуют:**
- JWT токен в заголовке `Authorization: Bearer {token}`
- Роль пользователя: `admin`

**Формат заголовка:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Ответ при отсутствии авторизации:**
- **401 Unauthorized** - токен не передан или невалиден
- **403 Forbidden** - пользователь не является администратором

---

# Аналитика и отчеты

Base URL: `/api/admin/reports`

---

## 1. Дневная статистика продаж

Получение статистики продаж по дням.

**Endpoint:** `GET /api/admin/reports/sales/daily`

### Параметры запроса (Query)

| Параметр | Тип | Обязательный | По умолчанию | Описание |
|----------|-----|--------------|--------------|----------|
| `days` | int | Нет | 30 | Количество дней назад от текущей даты |

### Пример запроса
```
GET /api/admin/reports/sales/daily?days=7
```

### Пример ответа (200 OK)
```json
[
  {
    "date": "2024-11-15T00:00:00Z",
    "ordersCount": 15,
    "totalRevenue": 12500.50,
    "itemsSold": 45,
    "averageOrderValue": 833.37
  },
  {
    "date": "2024-11-14T00:00:00Z",
    "ordersCount": 12,
    "totalRevenue": 9800.00,
    "itemsSold": 38,
    "averageOrderValue": 816.67
  }
]
```

### Модель ответа (DailySalesReportDTO)
```kotlin
data class DailySalesReportDTO(
    val date: String,              // ISO 8601 формат
    val ordersCount: Int,          // Количество заказов
    val totalRevenue: Double,      // Общая выручка
    val itemsSold: Int,            // Продано товаров
    val averageOrderValue: Double  // Средний чек
)
```

---

## 2. Отчет за период

Получение детальной статистики продаж за указанный период.

**Endpoint:** `GET /api/admin/reports/sales/period`

### Параметры запроса (Query)

| Параметр | Тип | Обязательный | По умолчанию | Описание |
|----------|-----|--------------|--------------|----------|
| `from` | DateTime | Нет | Месяц назад | Начало периода (YYYY-MM-DD) |
| `to` | DateTime | Нет | Сегодня | Конец периода (YYYY-MM-DD) |

### Пример запроса
```
GET /api/admin/reports/sales/period?from=2024-01-01&to=2024-12-31
```

### Пример ответа (200 OK)
```json
{
  "fromDate": "2024-01-01T00:00:00Z",
  "toDate": "2024-12-31T23:59:59Z",
  "totalOrders": 450,
  "totalRevenue": 375000.00,
  "totalItemsSold": 1250,
  "averageOrderValue": 833.33,
  "dailySales": [
    {
      "date": "2024-01-01T00:00:00Z",
      "ordersCount": 5,
      "totalRevenue": 4200.00,
      "itemsSold": 15,
      "averageOrderValue": 840.00
    }
  ]
}
```

### Модель ответа (PeriodSalesReportDTO)
```kotlin
data class PeriodSalesReportDTO(
    val fromDate: String,
    val toDate: String,
    val totalOrders: Int,
    val totalRevenue: Double,
    val totalItemsSold: Int,
    val averageOrderValue: Double,
    val dailySales: List<DailySalesReportDTO>
)
```

---

## 3. Помесячная выручка

Получение выручки с разбивкой по месяцам.

**Endpoint:** `GET /api/admin/reports/revenue/monthly`

### Параметры запроса (Query)

| Параметр | Тип | Обязательный | По умолчанию | Описание |
|----------|-----|--------------|--------------|----------|
| `months` | int | Нет | 12 | Количество месяцев назад |

### Пример запроса
```
GET /api/admin/reports/revenue/monthly?months=6
```

### Пример ответа (200 OK)
```json
[
  {
    "year": 2024,
    "month": 11,
    "monthName": "November",
    "ordersCount": 85,
    "totalRevenue": 67500.00,
    "itemsSold": 245
  },
  {
    "year": 2024,
    "month": 10,
    "monthName": "October",
    "ordersCount": 92,
    "totalRevenue": 73200.00,
    "itemsSold": 280
  }
]
```

### Модель ответа (MonthlyRevenueDTO)
```kotlin
data class MonthlyRevenueDTO(
    val year: Int,
    val month: Int,             // 1-12
    val monthName: String,      // Название месяца
    val ordersCount: Int,
    val totalRevenue: Double,
    val itemsSold: Int
)
```

---

## 4. Топ продаваемых товаров

Получение списка самых продаваемых товаров.

**Endpoint:** `GET /api/admin/reports/top-products`

### Параметры запроса (Query)

| Параметр | Тип | Обязательный | По умолчанию | Описание |
|----------|-----|--------------|--------------|----------|
| `limit` | int | Нет | 10 | Количество товаров в топе |
| `from` | DateTime | Нет | Месяц назад | Начало периода |
| `to` | DateTime | Нет | Сегодня | Конец периода |

### Пример запроса
```
GET /api/admin/reports/top-products?limit=5&from=2024-01-01&to=2024-12-31
```

### Пример ответа (200 OK)
```json
[
  {
    "productId": 15,
    "productName": "Смартфон Samsung Galaxy S23",
    "sku": "SAMS-S23-128GB",
    "totalQuantitySold": 145,
    "totalRevenue": 87000.00,
    "ordersCount": 120
  },
  {
    "productId": 7,
    "productName": "Ноутбук ASUS ROG",
    "sku": "ASUS-ROG-2024",
    "totalQuantitySold": 68,
    "totalRevenue": 102000.00,
    "ordersCount": 65
  }
]
```

### Модель ответа (TopProductDTO)
```kotlin
data class TopProductDTO(
    val productId: Int,
    val productName: String,
    val sku: String,                  // Артикул товара
    val totalQuantitySold: Int,       // Всего продано штук
    val totalRevenue: Double,         // Общая выручка
    val ordersCount: Int              // В скольких заказах
)
```

---

## 5. Продажи по категориям

Статистика продаж с группировкой по категориям товаров.

**Endpoint:** `GET /api/admin/reports/sales-by-category`

### Параметры запроса (Query)

| Параметр | Тип | Обязательный | По умолчанию | Описание |
|----------|-----|--------------|--------------|----------|
| `from` | DateTime | Нет | Месяц назад | Начало периода |
| `to` | DateTime | Нет | Сегодня | Конец периода |

### Пример запроса
```
GET /api/admin/reports/sales-by-category?from=2024-11-01&to=2024-11-30
```

### Пример ответа (200 OK)
```json
[
  {
    "categoryId": 3,
    "categoryName": "Смартфоны",
    "productsCount": 15,
    "totalQuantitySold": 245,
    "totalRevenue": 147000.00,
    "ordersCount": 180,
    "averagePrice": 600.00
  },
  {
    "categoryId": 5,
    "categoryName": "Ноутбуки",
    "productsCount": 8,
    "totalQuantitySold": 95,
    "totalRevenue": 142500.00,
    "ordersCount": 85,
    "averagePrice": 1500.00
  }
]
```

### Модель ответа (CategorySalesDTO)
```kotlin
data class CategorySalesDTO(
    val categoryId: Int,
    val categoryName: String,
    val productsCount: Int,           // Количество уникальных товаров
    val totalQuantitySold: Int,       // Всего продано единиц
    val totalRevenue: Double,         // Общая выручка
    val ordersCount: Int,             // Количество заказов
    val averagePrice: Double          // Средняя цена товара
)
```

---

## 6. Статистика по способам оплаты

Распределение заказов и выручки по способам оплаты.

**Endpoint:** `GET /api/admin/reports/payment-methods`

### Параметры запроса (Query)

| Параметр | Тип | Обязательный | По умолчанию | Описание |
|----------|-----|--------------|--------------|----------|
| `from` | DateTime | Нет | Месяц назад | Начало периода |
| `to` | DateTime | Нет | Сегодня | Конец периода |

### Пример запроса
```
GET /api/admin/reports/payment-methods?from=2024-11-01&to=2024-11-30
```

### Пример ответа (200 OK)
```json
[
  {
    "paymentMethod": "Card",
    "ordersCount": 320,
    "totalRevenue": 256000.00,
    "percentage": 72.5
  },
  {
    "paymentMethod": "Cash",
    "ordersCount": 98,
    "totalRevenue": 97000.00,
    "percentage": 27.5
  }
]
```

### Модель ответа (PaymentMethodStatsDTO)
```kotlin
data class PaymentMethodStatsDTO(
    val paymentMethod: String,    // "Card" или "Cash"
    val ordersCount: Int,
    val totalRevenue: Double,
    val percentage: Double        // Процент от общей выручки
)
```

---

# Dashboard

Общая статистика и алерты для административной панели.

---

## 7. Dashboard сводка

Получение полной сводки для главной страницы админ-панели.

**Endpoint:** `GET /api/admin/reports/dashboard`

### Параметры запроса
Отсутствуют

### Пример запроса
```
GET /api/admin/reports/dashboard
```

### Пример ответа (200 OK)
```json
{
  "totalOrders": 1250,
  "totalProducts": 156,
  "totalCategories": 12,
  "totalUsers": 450,
  "totalRevenue": 875000.00,
  "todayOrders": 15,
  "todayRevenue": 12500.00,
  "weekOrders": 85,
  "weekRevenue": 67500.00,
  "monthOrders": 320,
  "monthRevenue": 256000.00,
  "averageOrderValue": 700.00,
  "lowStockProductsCount": 8,
  "outOfStockProductsCount": 3,
  "pendingOrdersCount": 12,
  "ordersByStatus": {
    "Pending": 12,
    "Processing": 25,
    "Completed": 1200,
    "Cancelled": 13
  },
  "recentOrders": [
    {
      "id": 1245,
      "customerName": "Иван Иванов",
      "totalAmount": 1500.00,
      "status": "Pending",
      "createdAt": "2024-11-15T14:30:00Z"
    },
    {
      "id": 1244,
      "customerName": "Мария Петрова",
      "totalAmount": 850.00,
      "status": "Processing",
      "createdAt": "2024-11-15T13:15:00Z"
    }
  ]
}
```

### Модели ответа

**DashboardSummaryDTO:**
```kotlin
data class DashboardSummaryDTO(
    // Общая статистика
    val totalOrders: Int,
    val totalProducts: Int,
    val totalCategories: Int,
    val totalUsers: Int,
    val totalRevenue: Double,

    // За сегодня
    val todayOrders: Int,
    val todayRevenue: Double,

    // За неделю
    val weekOrders: Int,
    val weekRevenue: Double,

    // За месяц
    val monthOrders: Int,
    val monthRevenue: Double,

    // KPI
    val averageOrderValue: Double,          // Средний чек
    val lowStockProductsCount: Int,         // Товаров с остатком ≤10
    val outOfStockProductsCount: Int,       // Товаров с нулевым остатком
    val pendingOrdersCount: Int,            // Необработанных заказов

    // Статусы
    val ordersByStatus: Map<String, Int>,   // Заказы по статусам

    // Последние заказы
    val recentOrders: List<RecentOrderDTO>  // Последние 5 заказов
)

data class RecentOrderDTO(
    val id: Int,
    val customerName: String,
    val totalAmount: Double,
    val status: String,
    val createdAt: String
)
```

---

## 8. Алерты для Dashboard

Получение списка важных уведомлений для администратора.

**Endpoint:** `GET /api/admin/reports/alerts`

### Параметры запроса
Отсутствуют

### Пример запроса
```
GET /api/admin/reports/alerts
```

### Пример ответа (200 OK)
```json
[
  {
    "type": "warning",
    "message": "Товары с низким остатком",
    "count": 8
  },
  {
    "type": "error",
    "message": "Товары закончились на складе",
    "count": 3
  },
  {
    "type": "info",
    "message": "Необработанные заказы",
    "count": 12
  }
]
```

### Модель ответа (DashboardAlertDTO)
```kotlin
data class DashboardAlertDTO(
    val type: String,      // "warning", "info", "error"
    val message: String,   // Текст сообщения
    val count: Int         // Количество элементов
)
```

**Типы алертов:**
- `warning` - Предупреждение (низкий остаток товаров ≤10)
- `error` - Ошибка (товары закончились)
- `info` - Информация (необработанные заказы)

---

# Экспорт данных

Base URL: `/api/admin/export`

Все экспорты возвращают файлы для скачивания. **Важно:** Эти endpoint'ы возвращают файлы, а не JSON.

---

## 9. Экспорт товаров

**CSV:** `GET /api/admin/export/products/csv`
**Excel:** `GET /api/admin/export/products/excel`

### Параметры запроса
Отсутствуют

### Заголовки ответа
```
Content-Type: text/csv (для CSV)
Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet (для Excel)
Content-Disposition: attachment; filename="products_20241115.csv"
```

### Структура данных в файле
| Колонка | Описание |
|---------|----------|
| Id | ID товара |
| Name | Название |
| SKU | Артикул |
| Category | Категория |
| Price | Цена |
| StockQuantity | Остаток на складе |
| Description | Описание |

### Пример для Kotlin (скачивание файла)
```kotlin
suspend fun downloadProductsCsv(): ResponseBody {
    return api.downloadProductsCsv()
}

// Сохранение файла
fun saveFile(body: ResponseBody, filename: String) {
    val file = File(context.getExternalFilesDir(null), filename)
    file.outputStream().use { output ->
        body.byteStream().use { input ->
            input.copyTo(output)
        }
    }
}
```

---

## 10. Экспорт заказов

**CSV:** `GET /api/admin/export/orders/csv`
**Excel:** `GET /api/admin/export/orders/excel`

### Параметры запроса (Query)

| Параметр | Тип | Обязательный | По умолчанию | Описание |
|----------|-----|--------------|--------------|----------|
| `from` | DateTime | Нет | Месяц назад | Начало периода |
| `to` | DateTime | Нет | Сегодня | Конец периода |

### Пример запроса
```
GET /api/admin/export/orders/excel?from=2024-01-01&to=2024-12-31
```

### Структура данных в файле

**Для CSV:**
| Колонка | Описание |
|---------|----------|
| Id | ID заказа |
| CustomerName | Имя клиента |
| CustomerPhone | Телефон |
| DeliveryAddress | Адрес доставки |
| PaymentMethod | Способ оплаты |
| TotalAmount | Сумма |
| Status | Статус |
| CreatedAt | Дата создания |

**Для Excel (расширенная версия):**
| Колонка | Описание |
|---------|----------|
| OrderId | ID заказа |
| Customer | Имя клиента |
| Phone | Телефон |
| Address | Адрес |
| PaymentMethod | Способ оплаты |
| TotalAmount | Сумма |
| Status | Статус |
| CreatedAt | Дата создания |

---

## 11. Экспорт отчета по продажам

**CSV:** `GET /api/admin/export/sales-report/csv`
**Excel:** `GET /api/admin/export/sales-report/excel`

### Параметры запроса (Query)

| Параметр | Тип | Обязательный | По умолчанию | Описание |
|----------|-----|--------------|--------------|----------|
| `from` | DateTime | Нет | Месяц назад | Начало периода |
| `to` | DateTime | Нет | Сегодня | Конец периода |

### Пример запроса
```
GET /api/admin/export/sales-report/csv?from=2024-11-01&to=2024-11-30
```

### Структура данных в файле
| Колонка | Описание |
|---------|----------|
| Date | Дата |
| OrdersCount | Количество заказов |
| TotalRevenue | Выручка |
| ItemsSold | Товаров продано |
| AverageOrderValue | Средний чек |

---

## 12. Экспорт топ товаров

**CSV:** `GET /api/admin/export/top-products/csv`
**Excel:** `GET /api/admin/export/top-products/excel`

### Параметры запроса (Query)

| Параметр | Тип | Обязательный | По умолчанию | Описание |
|----------|-----|--------------|--------------|----------|
| `limit` | int | Нет | 10 | Количество товаров |
| `from` | DateTime | Нет | Месяц назад | Начало периода |
| `to` | DateTime | Нет | Сегодня | Конец периода |

### Пример запроса
```
GET /api/admin/export/top-products/excel?limit=20&from=2024-01-01&to=2024-12-31
```

### Структура данных в файле
| Колонка | Описание |
|---------|----------|
| ProductId | ID товара |
| ProductName | Название |
| TotalQuantitySold | Всего продано |
| TotalRevenue | Выручка |
| OrdersCount | В заказах |

---

## 13. Экспорт инвентаризации склада

**CSV:** `GET /api/admin/export/inventory/csv`
**Excel:** `GET /api/admin/export/inventory/excel`

### Параметры запроса
Отсутствуют

### Пример запроса
```
GET /api/admin/export/inventory/excel
```

### Структура данных в файле

**Для CSV:**
| Колонка | Описание |
|---------|----------|
| Id | ID товара |
| Name | Название |
| SKU | Артикул |
| Category | Категория |
| Price | Цена |
| StockQuantity | Остаток |
| TotalValue | Общая стоимость |
| Status | Статус |

**Для Excel (расширенная версия):**
| Колонка | Описание |
|---------|----------|
| ProductId | ID товара |
| ProductName | Название |
| SKU | Артикул |
| Category | Категория |
| Price | Цена за единицу |
| StockQuantity | Остаток на складе |
| TotalValue | Price × StockQuantity |
| Status | "Out of Stock" / "Low Stock" / "In Stock" |

**Статусы:**
- `Out of Stock` - остаток = 0
- `Low Stock` - остаток ≤ 10
- `In Stock` - остаток > 10

---

# Коды ошибок

## HTTP статус коды

| Код | Описание | Когда возникает |
|-----|----------|-----------------|
| 200 | OK | Успешный запрос |
| 400 | Bad Request | Неверные параметры запроса |
| 401 | Unauthorized | Токен не передан или невалиден |
| 403 | Forbidden | Недостаточно прав (не admin) |
| 404 | Not Found | Ресурс не найден |
| 500 | Internal Server Error | Ошибка сервера |

## Формат ошибки

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "days": ["The value must be greater than 0."]
  }
}
```

---

# Примеры для Kotlin/Android

## Retrofit интерфейс

```kotlin
interface AdminApiService {

    // Аналитика
    @GET("api/admin/reports/sales/daily")
    suspend fun getDailySales(
        @Query("days") days: Int = 30
    ): List<DailySalesReportDTO>

    @GET("api/admin/reports/sales/period")
    suspend fun getPeriodSales(
        @Query("from") from: String?,
        @Query("to") to: String?
    ): PeriodSalesReportDTO

    @GET("api/admin/reports/revenue/monthly")
    suspend fun getMonthlyRevenue(
        @Query("months") months: Int = 12
    ): List<MonthlyRevenueDTO>

    @GET("api/admin/reports/top-products")
    suspend fun getTopProducts(
        @Query("limit") limit: Int = 10,
        @Query("from") from: String?,
        @Query("to") to: String?
    ): List<TopProductDTO>

    @GET("api/admin/reports/sales-by-category")
    suspend fun getSalesByCategory(
        @Query("from") from: String?,
        @Query("to") to: String?
    ): List<CategorySalesDTO>

    @GET("api/admin/reports/payment-methods")
    suspend fun getPaymentMethodStats(
        @Query("from") from: String?,
        @Query("to") to: String?
    ): List<PaymentMethodStatsDTO>

    // Dashboard
    @GET("api/admin/reports/dashboard")
    suspend fun getDashboard(): DashboardSummaryDTO

    @GET("api/admin/reports/alerts")
    suspend fun getAlerts(): List<DashboardAlertDTO>

    // Экспорт (скачивание файлов)
    @GET("api/admin/export/products/csv")
    @Streaming
    suspend fun downloadProductsCsv(): ResponseBody

    @GET("api/admin/export/products/excel")
    @Streaming
    suspend fun downloadProductsExcel(): ResponseBody

    @GET("api/admin/export/orders/csv")
    @Streaming
    suspend fun downloadOrdersCsv(
        @Query("from") from: String?,
        @Query("to") to: String?
    ): ResponseBody

    @GET("api/admin/export/orders/excel")
    @Streaming
    suspend fun downloadOrdersExcel(
        @Query("from") from: String?,
        @Query("to") to: String?
    ): ResponseBody

    @GET("api/admin/export/sales-report/csv")
    @Streaming
    suspend fun downloadSalesReportCsv(
        @Query("from") from: String?,
        @Query("to") to: String?
    ): ResponseBody

    @GET("api/admin/export/sales-report/excel")
    @Streaming
    suspend fun downloadSalesReportExcel(
        @Query("from") from: String?,
        @Query("to") to: String?
    ): ResponseBody

    @GET("api/admin/export/top-products/csv")
    @Streaming
    suspend fun downloadTopProductsCsv(
        @Query("limit") limit: Int = 10,
        @Query("from") from: String?,
        @Query("to") to: String?
    ): ResponseBody

    @GET("api/admin/export/top-products/excel")
    @Streaming
    suspend fun downloadTopProductsExcel(
        @Query("limit") limit: Int = 10,
        @Query("from") from: String?,
        @Query("to") to: String?
    ): ResponseBody

    @GET("api/admin/export/inventory/csv")
    @Streaming
    suspend fun downloadInventoryCsv(): ResponseBody

    @GET("api/admin/export/inventory/excel")
    @Streaming
    suspend fun downloadInventoryExcel(): ResponseBody
}
```

## Data классы (Models)

```kotlin
data class DailySalesReportDTO(
    val date: String,
    val ordersCount: Int,
    val totalRevenue: Double,
    val itemsSold: Int,
    val averageOrderValue: Double
)

data class PeriodSalesReportDTO(
    val fromDate: String,
    val toDate: String,
    val totalOrders: Int,
    val totalRevenue: Double,
    val totalItemsSold: Int,
    val averageOrderValue: Double,
    val dailySales: List<DailySalesReportDTO>
)

data class MonthlyRevenueDTO(
    val year: Int,
    val month: Int,
    val monthName: String,
    val ordersCount: Int,
    val totalRevenue: Double,
    val itemsSold: Int
)

data class TopProductDTO(
    val productId: Int,
    val productName: String,
    val sku: String,
    val totalQuantitySold: Int,
    val totalRevenue: Double,
    val ordersCount: Int
)

data class CategorySalesDTO(
    val categoryId: Int,
    val categoryName: String,
    val productsCount: Int,
    val totalQuantitySold: Int,
    val totalRevenue: Double,
    val ordersCount: Int,
    val averagePrice: Double
)

data class PaymentMethodStatsDTO(
    val paymentMethod: String,
    val ordersCount: Int,
    val totalRevenue: Double,
    val percentage: Double
)

data class DashboardSummaryDTO(
    val totalOrders: Int,
    val totalProducts: Int,
    val totalCategories: Int,
    val totalUsers: Int,
    val totalRevenue: Double,
    val todayOrders: Int,
    val todayRevenue: Double,
    val weekOrders: Int,
    val weekRevenue: Double,
    val monthOrders: Int,
    val monthRevenue: Double,
    val averageOrderValue: Double,
    val lowStockProductsCount: Int,
    val outOfStockProductsCount: Int,
    val pendingOrdersCount: Int,
    val ordersByStatus: Map<String, Int>,
    val recentOrders: List<RecentOrderDTO>
)

data class RecentOrderDTO(
    val id: Int,
    val customerName: String,
    val totalAmount: Double,
    val status: String,
    val createdAt: String
)

data class DashboardAlertDTO(
    val type: String,
    val message: String,
    val count: Int
)
```

## Interceptor для JWT токена

```kotlin
class AuthInterceptor(private val tokenProvider: () -> String?) : Interceptor {
    override fun intercept(chain: Interceptor.Chain): Response {
        val request = chain.request()
        val token = tokenProvider()

        val newRequest = if (token != null) {
            request.newBuilder()
                .header("Authorization", "Bearer $token")
                .build()
        } else {
            request
        }

        return chain.proceed(newRequest)
    }
}
```

## Настройка Retrofit

```kotlin
val okHttpClient = OkHttpClient.Builder()
    .addInterceptor(AuthInterceptor {
        // Получение токена из SharedPreferences или другого хранилища
        sharedPreferences.getString("jwt_token", null)
    })
    .build()

val retrofit = Retrofit.Builder()
    .baseUrl("https://your-api.com/")
    .client(okHttpClient)
    .addConverterFactory(GsonConverterFactory.create())
    .build()

val adminApi = retrofit.create(AdminApiService::class.java)
```

## Примеры использования в ViewModel

```kotlin
class DashboardViewModel(
    private val adminApi: AdminApiService
) : ViewModel() {

    private val _dashboardData = MutableLiveData<DashboardSummaryDTO>()
    val dashboardData: LiveData<DashboardSummaryDTO> = _dashboardData

    private val _alerts = MutableLiveData<List<DashboardAlertDTO>>()
    val alerts: LiveData<List<DashboardAlertDTO>> = _alerts

    private val _isLoading = MutableLiveData<Boolean>()
    val isLoading: LiveData<Boolean> = _isLoading

    private val _error = MutableLiveData<String>()
    val error: LiveData<String> = _error

    fun loadDashboard() {
        viewModelScope.launch {
            _isLoading.value = true
            try {
                val dashboard = adminApi.getDashboard()
                _dashboardData.value = dashboard

                val alerts = adminApi.getAlerts()
                _alerts.value = alerts

            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }
}
```

## Пример скачивания файла

```kotlin
class ExportRepository(
    private val adminApi: AdminApiService,
    private val context: Context
) {

    suspend fun downloadProductsExcel(): Result<File> = withContext(Dispatchers.IO) {
        try {
            val response = adminApi.downloadProductsExcel()
            val filename = "products_${System.currentTimeMillis()}.xlsx"
            val file = File(context.getExternalFilesDir(null), filename)

            file.outputStream().use { output ->
                response.byteStream().use { input ->
                    input.copyTo(output)
                }
            }

            Result.success(file)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun downloadSalesReportCsv(
        from: String?,
        to: String?
    ): Result<File> = withContext(Dispatchers.IO) {
        try {
            val response = adminApi.downloadSalesReportCsv(from, to)
            val filename = "sales_report_${System.currentTimeMillis()}.csv"
            val file = File(context.getExternalFilesDir(null), filename)

            file.outputStream().use { output ->
                response.byteStream().use { input ->
                    input.copyTo(output)
                }
            }

            Result.success(file)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
```

## Обработка ошибок

```kotlin
sealed class ApiResult<out T> {
    data class Success<T>(val data: T) : ApiResult<T>()
    data class Error(val code: Int, val message: String) : ApiResult<Nothing>()
    object Loading : ApiResult<Nothing>()
}

suspend fun <T> safeApiCall(apiCall: suspend () -> T): ApiResult<T> {
    return try {
        ApiResult.Success(apiCall())
    } catch (e: HttpException) {
        when (e.code()) {
            401 -> ApiResult.Error(401, "Не авторизован")
            403 -> ApiResult.Error(403, "Недостаточно прав")
            404 -> ApiResult.Error(404, "Данные не найдены")
            else -> ApiResult.Error(e.code(), e.message())
        }
    } catch (e: Exception) {
        ApiResult.Error(0, e.message ?: "Неизвестная ошибка")
    }
}

// Использование
viewModelScope.launch {
    val result = safeApiCall {
        adminApi.getDashboard()
    }

    when (result) {
        is ApiResult.Success -> {
            _dashboardData.value = result.data
        }
        is ApiResult.Error -> {
            _error.value = result.message
        }
        is ApiResult.Loading -> {
            // Показываем прогресс
        }
    }
}
```

---

## Форматирование дат

Все даты в запросах должны быть в формате **ISO 8601**: `YYYY-MM-DD`

Пример:
```kotlin
import java.time.LocalDate
import java.time.format.DateTimeFormatter

val formatter = DateTimeFormatter.ISO_LOCAL_DATE
val from = LocalDate.of(2024, 1, 1).format(formatter)  // "2024-01-01"
val to = LocalDate.now().format(formatter)             // "2024-11-15"

adminApi.getPeriodSales(from, to)
```

---

## Важные примечания

1. **Все административные endpoint'ы требуют роль `admin`**
2. **Отмененные заказы (`Status = "Cancelled"`) исключаются из статистики**
3. **Все суммы в формате `Double` с точностью до копеек**
4. **Даты возвращаются в UTC формате (ISO 8601)**
5. **Файлы экспорта возвращают `ResponseBody`, а не JSON**
6. **Низкий остаток определяется как StockQuantity ≤ 10**
7. **По умолчанию период "месяц назад" = текущая дата - 30 дней**

---

## Тестирование

### Получение JWT токена администратора

```http
POST /register
Content-Type: application/json

{
  "login": "admin",
  "password": "admin123"
}
```

```http
POST /login
Content-Type: application/json

{
  "login": "admin",
  "password": "admin123"
}
```

**Ответ:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "login": "admin",
    "role": "admin"
  }
}
```

Используйте полученный токен в заголовке `Authorization: Bearer {token}`

---

## Контакты для вопросов

При возникновении вопросов по интеграции обращайтесь к backend-разработчику.
