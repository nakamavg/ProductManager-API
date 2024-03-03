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
});