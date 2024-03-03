# Documentación Técnica de la API

## Endpoints de Autenticación

### POST /api/auth/login
Endpoint para autenticación de usuarios.

**Request Body:**
```json
{
    "email": "string",
    "password": "string"
}
```

**Response (200 OK):**
```json
{
    "token": "string",
    "expiresIn": "number",
    "usuario": {
        "id": "number",
        "nombre": "string",
        "email": "string",
        "rol": "string"
    }
}
```

**Posibles errores:**
- 400 Bad Request: Credenciales inválidas
- 401 Unauthorized: Usuario no autorizado

### POST /api/auth/register
Registro de nuevos usuarios.

**Request Body:**
```json
{
    "nombre": "string",
    "email": "string",
    "password": "string",
    "rol": "string"
}
```

**Response (201 Created):**
```json
{
    "id": "number",
    "nombre": "string",
    "email": "string",
    "rol": "string"
}
```

## Endpoints de Productos

### GET /api/productos
Obtiene lista de productos con paginación.

**Query Parameters:**
- page (opcional): Número de página
- pageSize (opcional): Tamaño de página
- search (opcional): Término de búsqueda
- sortBy (opcional): Campo para ordenar
- sortDirection (opcional): asc/desc

**Response (200 OK):**
```json
{
    "items": [
        {
            "id": "number",
            "nombre": "string",
            "descripcion": "string",
            "precio": "number",
            "categoria": "string",
            "stock": "number"
        }
    ],
    "total": "number",
    "page": "number",
    "pageSize": "number"
}
```

### GET /api/productos/{id}
Obtiene un producto específico.

**Response (200 OK):**
```json
{
    "id": "number",
    "nombre": "string",
    "descripcion": "string",
    "precio": "number",
    "categoria": "string",
    "stock": "number",
    "detallesTecnicos": "object"
}
```

### POST /api/productos
Crea un nuevo producto (requiere rol Admin).

**Request Body:**
```json
{
    "nombre": "string",
    "descripcion": "string",
    "precio": "number",
    "categoria": "string",
    "stock": "number",
    "detallesTecnicos": "object"
}
```

### PUT /api/productos/{id}
Actualiza un producto existente (requiere rol Admin).

**Request Body:** Similar al POST

### DELETE /api/productos/{id}
Elimina un producto (requiere rol Admin).

## Sistema de Autorización

La API implementa un sistema de autorización basado en JWT con los siguientes roles:
- **Admin**: Acceso total
- **Usuario**: Solo lectura de productos
- **Vendedor**: Lectura y modificación de productos

### Headers Requeridos
Para endpoints protegidos:
```
Authorization: Bearer {token}
```

## Paginación y Filtrado

La API soporta paginación y filtrado avanzado:

### Parámetros de Paginación
- **page**: Número de página (default: 1)
- **pageSize**: Elementos por página (default: 10, max: 100)

### Filtrado
- **search**: Búsqueda por nombre o descripción
- **categoria**: Filtro por categoría
- **precioMin/precioMax**: Rango de precios

### Ordenamiento
- **sortBy**: Campo para ordenar
- **sortDirection**: asc/desc

## Manejo de Errores

La API utiliza los siguientes códigos de estado HTTP:

- **200**: Éxito
- **201**: Recurso creado
- **400**: Error en la petición
- **401**: No autorizado
- **403**: Prohibido
- **404**: Recurso no encontrado
- **500**: Error interno del servidor

Formato de respuesta de error:
```json
{
    "error": {
        "code": "string",
        "message": "string",
        "details": "object"
    }
}
```

## Rate Limiting

La API implementa límites de tasa:
- 100 peticiones por minuto para usuarios autenticados
- 30 peticiones por minuto para usuarios no autenticados