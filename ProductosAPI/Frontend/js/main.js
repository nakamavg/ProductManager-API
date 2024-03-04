// Funciones generales y controlador principal de la aplicación

// Mostrar una vista específica y ocultar las demás
function mostrarVista(vistaId) {
    $('#loginView, #registerView, #productosView, #productoFormView').hide();
    $(`#${vistaId}`).show();
}

// Mostrar mensaje de alerta
function mostrarAlerta(mensaje, tipo) {
    const alerta = `
    <div class="alert alert-${tipo} alert-dismissible fade show" role="alert">
        ${mensaje}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    </div>`;
    
    $('#alertContainer').html(alerta);
    
    // Eliminar después de 5 segundos
    setTimeout(function() {
        $('.alert').alert('close');
    }, 5000);
}

// Actualizar UI según estado de autenticación
function actualizarUIAutenticacion() {
    if (authService.isAuthenticated()) {
        // Usuario autenticado
        const usuario = authService.getCurrentUser();
        
        $('#userNameDisplay').text(usuario.name);
        $('#loginNavItem, #registerNavItem').hide();
        $('#logoutNavItem').show();
        
        // Mostrar opciones según el rol
        if (authService.isAdmin()) {
            $('.admin-only').show();
        } else {
            $('.admin-only').hide();
        }
        
        // Mostrar lista de productos por defecto
        mostrarVista('productosView');
        productosUI.cargarProductos();
    } else {
        // Usuario no autenticado
        $('#loginNavItem, #registerNavItem').show();
        $('#logoutNavItem').hide();
        $('.admin-only').hide();
        
        // Mostrar login por defecto
        mostrarVista('loginView');
    }
}

// Inicialización cuando el documento está listo
$(document).ready(function() {
    
    // Verificar autenticación al cargar la página
    actualizarUIAutenticacion();
    
    // Eventos de navegación
    $('#loginLink').click(function(e) {
        e.preventDefault();
        mostrarVista('loginView');
    });
    
    $('#registerLink').click(function(e) {
        e.preventDefault();
        mostrarVista('registerView');
    });
    
    $('#logoutLink').click(function(e) {
        e.preventDefault();
        authService.clearAuthData();
        actualizarUIAutenticacion();
        mostrarAlerta('Sesión cerrada correctamente', 'success');
    });
    
    $('#listaProductosLink').click(function(e) {
        e.preventDefault();
        
        if (authService.isAuthenticated()) {
            mostrarVista('productosView');
            productosUI.cargarProductos();
        } else {
            mostrarAlerta('Debe iniciar sesión para ver los productos', 'warning');
            mostrarVista('loginView');
        }
    });
    
    $('#crearProductoLink').click(function(e) {
        e.preventDefault();
        
        if (authService.isAdmin()) {
            productosUI.prepararNuevoProducto();
        } else {
            mostrarAlerta('No tiene permisos para crear productos', 'warning');
        }
    });
    
    // Formulario de login
    $('#loginForm').submit(function(e) {
        e.preventDefault();
        
        const email = $('#loginEmail').val();
        const password = $('#loginPassword').val();
        
        authService.login(email, password)
            .done(function(data) {
                authService.setAuthData(data);
                actualizarUIAutenticacion();
                mostrarAlerta('Inicio de sesión exitoso', 'success');
            })
            .fail(function(error) {
                let mensaje = 'Error al iniciar sesión';
                
                if (error.status === 401) {
                    mensaje = 'Credenciales incorrectas';
                } else if (error.responseText) {
                    mensaje += `: ${error.responseText}`;
                }
                
                mostrarAlerta(mensaje, 'danger');
            });
    });
    
    // Formulario de registro
    $('#registerForm').submit(function(e) {
        e.preventDefault();
        
        const userData = {
            nombre: $('#registerNombre').val(),
            login: $('#registerLogin').val(),
            email: $('#registerEmail').val(),
            password: $('#registerPassword').val(),
            rol: parseInt($('#registerRol').val())
        };
        
        authService.register(userData)
            .done(function(data) {
                mostrarAlerta('Registro exitoso. Ahora puede iniciar sesión', 'success');
                mostrarVista('loginView');
                
                // Pre-llenar email para login
                $('#loginEmail').val(userData.email);
            })
            .fail(function(error) {
                let mensaje = 'Error al registrar usuario';
                
                if (error.responseJSON && error.responseJSON.message) {
                    mensaje += `: ${error.responseJSON.message}`;
                } else if (error.responseText) {
                    mensaje += `: ${error.responseText}`;
                }
                
                mostrarAlerta(mensaje, 'danger');
            });
    });
    
    // Formulario de producto
    $('#productoForm').submit(function(e) {
        e.preventDefault();
        
        if (!authService.isAdmin()) {
            mostrarAlerta('No tiene permisos para esta acción', 'danger');
            return;
        }
        
        productosUI.guardarProducto();
    });
    
    // Botón cancelar en formulario de producto
    $('#btnCancelarProducto').click(function() {
        mostrarVista('productosView');
    });
    
    // Botones para ordenar por nombre o fecha
    $('#btnOrdenarNombre').click(function() {
        productosUI.cambiarOrden('Nombre');
    });
    
    $('#btnOrdenarFecha').click(function() {
        productosUI.cambiarOrden('FechaCreacion');
    });

    // Configuración de filtros avanzados
    $('#filtrosAvanzados').append(`
        <div class="row mb-3">
            <div class="col-md-3">
                <label for="precioMinimo">Precio Mínimo</label>
                <input type="number" class="form-control" id="precioMinimo" min="0">
            </div>
            <div class="col-md-3">
                <label for="precioMaximo">Precio Máximo</label>
                <input type="number" class="form-control" id="precioMaximo" min="0">
            </div>
            <div class="col-md-3">
                <label for="categoriaFiltro">Categoría</label>
                <select class="form-control" id="categoriaFiltro">
                    <option value="">Todas</option>
                    <option value="electronica">Electrónica</option>
                    <option value="ropa">Ropa</option>
                    <option value="hogar">Hogar</option>
                </select>
            </div>
            <div class="col-md-3">
                <label>&nbsp;</label>
                <button class="btn btn-primary w-100" id="aplicarFiltros">
                    Aplicar Filtros
                </button>
            </div>
        </div>
    `);

    // Evento para aplicar filtros
    $('#aplicarFiltros').click(function() {
        productosUI.aplicarFiltros();
    });

    // Añadir métodos de filtrado al objeto productosUI
    Object.assign(productosUI, {
        filtrosActuales: {
            precioMinimo: null,
            precioMaximo: null,
            categoria: '',
            busqueda: ''
        },

        aplicarFiltros: function() {
            this.filtrosActuales = {
                precioMinimo: $('#precioMinimo').val() || null,
                precioMaximo: $('#precioMaximo').val() || null,
                categoria: $('#categoriaFiltro').val(),
                busqueda: $('#searchProductos').val()
            };
            this.paginaActual = 1;
            this.cargarProductos();
        },

        limpiarFiltros: function() {
            $('#precioMinimo').val('');
            $('#precioMaximo').val('');
            $('#categoriaFiltro').val('');
            $('#searchProductos').val('');
            this.filtrosActuales = {
                precioMinimo: null,
                precioMaximo: null,
                categoria: '',
                busqueda: ''
            };
            this.cargarProductos();
        }
    });

    // Agregar botón para limpiar filtros
    $('#filtrosContainer').append(`
        <button class="btn btn-secondary" id="limpiarFiltros">
            Limpiar Filtros
        </button>
    `);

    $('#limpiarFiltros').click(function() {
        productosUI.limpiarFiltros();
    });
});