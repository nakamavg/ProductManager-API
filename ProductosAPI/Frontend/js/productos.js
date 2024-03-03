// Gestión de productos
const productosService = {
    apiUrl: '/api',  // Actualizado para usar la ruta relativa a través del proxy Nginx
    
    // Obtener lista de productos con paginación
    obtenerProductos: function(pagina = 1, registrosPorPagina = 10, ordenarPor = 'Id', ascendente = true) {
        return $.ajax({
            url: `${this.apiUrl}/productos`,
            type: 'GET',
            data: {
                pagina: pagina,
                registrosPorPagina: registrosPorPagina,
                ordenarPor: ordenarPor,
                ascendente: ascendente
            }
        });
    },
    
    // Obtener un producto específico por ID
    obtenerProductoPorId: function(id) {
        return $.ajax({
            url: `${this.apiUrl}/productos/${id}`,
            type: 'GET'
        });
    },
    
    // Crear un nuevo producto
    crearProducto: function(producto) {
        return $.ajax({
            url: `${this.apiUrl}/productos`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(producto)
        });
    },
    
    // Actualizar un producto existente
    actualizarProducto: function(id, producto) {
        return $.ajax({
            url: `${this.apiUrl}/productos/${id}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(producto)
        });
    },
    
    // Eliminar un producto (desactivación lógica)
    eliminarProducto: function(id) {
        return $.ajax({
            url: `${this.apiUrl}/productos/${id}`,
            type: 'DELETE'
        });
    }
};

// Funciones para UI de productos
const productosUI = {
    // Variables para control de paginación y ordenación
    paginaActual: 1,
    registrosPorPagina: 10,
    ordenarPor: 'Id',
    ascendente: true,
    
    // Cargar productos en la tabla
    cargarProductos: function() {
        const self = this;
        
        // Mostrar indicador de carga
        $('#productosTableBody').html('<tr><td colspan="7" class="text-center">Cargando productos...</td></tr>');
        
        productosService.obtenerProductos(
            this.paginaActual, 
            this.registrosPorPagina,
            this.ordenarPor,
            this.ascendente
        )
        .done(function(resultado) {
            self.mostrarProductos(resultado);
            self.generarPaginacion(resultado);
        })
        .fail(function(error) {
            mostrarAlerta('Error al cargar los productos', 'danger');
            console.error('Error al cargar productos:', error);
        });
    },
    
    // Mostrar productos en la tabla
    mostrarProductos: function(resultado) {
        const productos = resultado.registros;
        let html = '';
        
        if (productos.length === 0) {
            html = '<tr><td colspan="7" class="text-center">No hay productos disponibles</td></tr>';
        } else {
            productos.forEach(function(producto) {
                const fecha = new Date(producto.fechaCreacion).toLocaleDateString();
                const dimensiones = `${producto.alto} × ${producto.ancho} × ${producto.profundidad}`;
                
                html += `
                <tr>
                    <td>${producto.id}</td>
                    <td>${producto.nombre}</td>
                    <td>${dimensiones}</td>
                    <td>${producto.volumen.toFixed(2)}</td>
                    <td>${producto.peso.toFixed(2)} kg</td>
                    <td>${fecha}</td>
                    <td>
                        <button class="btn btn-sm btn-info action-btn btn-ver" data-id="${producto.id}">
                            <i class="fas fa-eye"></i>
                        </button>`;
                
                // Mostrar botones de editar/eliminar solo para administradores
                if (authService.isAdmin()) {
                    html += `
                        <button class="btn btn-sm btn-primary action-btn btn-editar" data-id="${producto.id}">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-sm btn-danger action-btn btn-eliminar" data-id="${producto.id}">
                            <i class="fas fa-trash"></i>
                        </button>`;
                }
                
                html += `
                    </td>
                </tr>`;
            });
        }
        
        $('#productosTableBody').html(html);
        
        // Configurar eventos para botones
        $('.btn-ver').click(function() {
            const id = $(this).data('id');
            productosUI.verProducto(id);
        });
        
        $('.btn-editar').click(function() {
            const id = $(this).data('id');
            productosUI.cargarProductoParaEditar(id);
        });
        
        $('.btn-eliminar').click(function() {
            const id = $(this).data('id');
            productosUI.confirmarEliminar(id);
        });
    },
    
    // Generar controles de paginación
    generarPaginacion: function(resultado) {
        let html = '';
        const totalPaginas = resultado.totalPaginas;
        
        // Botón anterior
        html += `
        <li class="page-item ${this.paginaActual === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${this.paginaActual - 1}">&laquo;</a>
        </li>`;
        
        // Páginas individuales
        for (let i = 1; i <= totalPaginas; i++) {
            html += `
            <li class="page-item ${i === this.paginaActual ? 'active' : ''}">
                <a class="page-link" href="#" data-page="${i}">${i}</a>
            </li>`;
        }
        
        // Botón siguiente
        html += `
        <li class="page-item ${this.paginaActual === totalPaginas ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${this.paginaActual + 1}">&raquo;</a>
        </li>`;
        
        $('#paginacion').html(html);
        
        // Configurar eventos de paginación
        $('#paginacion a.page-link').click(function(e) {
            e.preventDefault();
            const pagina = $(this).data('page');
            
            if (pagina >= 1 && pagina <= totalPaginas) {
                productosUI.paginaActual = pagina;
                productosUI.cargarProductos();
            }
        });
    },
    
    // Cargar un producto para editar
    cargarProductoParaEditar: function(id) {
        productosService.obtenerProductoPorId(id)
            .done(function(producto) {
                // Llenar el formulario con los datos del producto
                $('#productoId').val(producto.id);
                $('#productoNombre').val(producto.nombre);
                $('#productoAlto').val(producto.alto);
                $('#productoAncho').val(producto.ancho);
                $('#productoProfundidad').val(producto.profundidad);
                $('#productoPeso').val(producto.peso);
                
                // Cambiar título y mostrar formulario
                $('#productoFormTitle').text('Editar Producto');
                mostrarVista('productoFormView');
            })
            .fail(function(error) {
                mostrarAlerta('Error al cargar el producto', 'danger');
                console.error('Error al cargar producto:', error);
            });
    },
    
    // Ver detalles de un producto (alternativa a editar para usuarios normales)
    verProducto: function(id) {
        productosService.obtenerProductoPorId(id)
            .done(function(producto) {
                // Crear una ventana modal con detalles del producto
                const fecha = new Date(producto.fechaCreacion).toLocaleDateString();
                const modal = `
                <div class="modal fade" id="detalleProductoModal" tabindex="-1">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Detalles del Producto</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3">
                                    <strong>Nombre:</strong> ${producto.nombre}
                                </div>
                                <div class="mb-3">
                                    <strong>Dimensiones:</strong> ${producto.alto} × ${producto.ancho} × ${producto.profundidad}
                                </div>
                                <div class="mb-3">
                                    <strong>Volumen:</strong> ${producto.volumen.toFixed(2)}
                                </div>
                                <div class="mb-3">
                                    <strong>Peso:</strong> ${producto.peso.toFixed(2)} kg
                                </div>
                                <div class="mb-3">
                                    <strong>Fecha de creación:</strong> ${fecha}
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                            </div>
                        </div>
                    </div>
                </div>
                `;
                
                // Añadir modal al DOM y mostrarlo
                $('body').append(modal);
                $('#detalleProductoModal').modal('show');
                
                // Eliminar modal del DOM cuando se cierre
                $('#detalleProductoModal').on('hidden.bs.modal', function() {
                    $(this).remove();
                });
            })
            .fail(function(error) {
                mostrarAlerta('Error al cargar los detalles del producto', 'danger');
            });
    },
    
    // Confirmar eliminación de producto
    confirmarEliminar: function(id) {
        if (confirm('¿Está seguro de que desea eliminar este producto?')) {
            productosService.eliminarProducto(id)
                .done(function() {
                    mostrarAlerta('Producto eliminado correctamente', 'success');
                    productosUI.cargarProductos();
                })
                .fail(function(error) {
                    mostrarAlerta('Error al eliminar el producto', 'danger');
                    console.error('Error al eliminar producto:', error);
                });
        }
    },
    
    // Preparar formulario para crear un nuevo producto
    prepararNuevoProducto: function() {
        // Limpiar formulario
        $('#productoId').val('');
        $('#productoNombre').val('');
        $('#productoAlto').val('');
        $('#productoAncho').val('');
        $('#productoProfundidad').val('');
        $('#productoPeso').val('');
        
        // Cambiar título y mostrar formulario
        $('#productoFormTitle').text('Crear Producto');
        mostrarVista('productoFormView');
    },
    
    // Guardar un producto (crear o actualizar)
    guardarProducto: function() {
        const id = $('#productoId').val();
        const esNuevo = id === '';
        
        const producto = {
            nombre: $('#productoNombre').val(),
            alto: parseFloat($('#productoAlto').val()),
            ancho: parseFloat($('#productoAncho').val()),
            profundidad: parseFloat($('#productoProfundidad').val()),
            peso: parseFloat($('#productoPeso').val())
        };
        
        const promise = esNuevo 
            ? productosService.crearProducto(producto)
            : productosService.actualizarProducto(id, producto);
        
        promise
            .done(function() {
                mostrarAlerta(`Producto ${esNuevo ? 'creado' : 'actualizado'} correctamente`, 'success');
                mostrarVista('productosView');
                productosUI.cargarProductos();
            })
            .fail(function(error) {
                let mensaje = `Error al ${esNuevo ? 'crear' : 'actualizar'} el producto`;
                
                // Intentar extraer mensaje de error específico
                if (error.responseJSON && error.responseJSON.message) {
                    mensaje += `: ${error.responseJSON.message}`;
                } else if (error.responseText) {
                    mensaje += `: ${error.responseText}`;
                }
                
                mostrarAlerta(mensaje, 'danger');
                console.error(`Error al ${esNuevo ? 'crear' : 'actualizar'} producto:`, error);
            });
    },
    
    // Cambiar orden de los productos
    cambiarOrden: function(campo) {
        if (this.ordenarPor === campo) {
            // Si ya estamos ordenando por este campo, invertir dirección
            this.ascendente = !this.ascendente;
        } else {
            // Si es un nuevo campo, ordenar ascendente por defecto
            this.ordenarPor = campo;
            this.ascendente = true;
        }
        
        // Recargar con nuevo orden
        this.cargarProductos();
    }
};