using apiFestivos.Aplicacion.Servicios;
using apiFestivos.Core.Interfaces.Repositorios;
using apiFestivos.Dominio.DTOs;
using apiFestivos.Dominio.Entidades;
using Microsoft.Win32.SafeHandles;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace apiFestivos.Test
{

    /// <summary>
    /// Clase de prueba para el método EsFestivo del servicio FestivoServicio
    /// </summary>
    public class FestivoServicioTest
    {

        /// <summary>
        /// Prueba positiva: Verifica que EsFestivo retorne true cuando la fecha coincide con un festivo
        /// </summary>
        [Fact]
        public async Task EsFestivo_RetornaTrue()
        {

            // Arrange: se simula un repositorio que retorna Año Nuevo como festivo
          
            var mockRepo = new Mock<IFestivoRepositorio>();
            mockRepo.Setup(r => r.ObtenerTodos()).ReturnsAsync(new List<Festivo>
            {
                new Festivo { IdTipo = 1, Nombre = "Año Nuevo", Dia = 1, Mes = 1 }
            });

            var servicio = new FestivoServicio(mockRepo.Object);
            var fecha = new DateTime(2025, 1, 1); // Año nuevo

            // Act: se llama al método EsFestivo
            var resultado = await servicio.EsFestivo(fecha);

            // Assert: se espera que el resultado sea true
            Assert.True(resultado);
        }

        /// <summary>
        /// Prueba negativa: Verifica que EsFestivo retorne false si la fecha no es festiva
        /// </summary>

        [Fact]
        public async Task EsFestivo_RetornaFalse()
        {

            // Arrange: se simula que solo existe Año Nuevo como festivo
            var mockRepo = new Mock<IFestivoRepositorio>();
            mockRepo.Setup(r => r.ObtenerTodos()).ReturnsAsync(new List<Festivo>
            {
                new Festivo { IdTipo = 1, Nombre = "Año Nuevo", Dia = 1, Mes = 1 }
            });

            var servicio = new FestivoServicio(mockRepo.Object);
            var fecha = new DateTime(2025, 2, 15); // No festivo

            // Act: se llama al método EsFestivo
            var resultado = await servicio.EsFestivo(fecha);

            // Assert: se espera que el resultado sea false
            Assert.False(resultado);
        }
    }

    /// <summary>
    /// Clase de pruebas para el método privado ObtenerFestivo de FestivoServicio
    /// </summary>
    public class FestivoServicio_obternerFest_Test
    {

        /// <summary>
        /// Método auxiliar para crear una instancia del servicio con un repositorio simulado
        /// </summary>
        private FestivoServicio CrearServicio()
        {
            var mockRepo = new Mock<IFestivoRepositorio>();
            return new FestivoServicio(mockRepo.Object);

        }

        /// <summary>
        /// Verifica que un festivo fijo (tipo 1) retorne la fecha exacta definida
        /// </summary>

        [Fact]
        public void ObtenerFestivo_Tipo1()
        {

            var servicio = CrearServicio();
            var festivo = new Festivo
            {
                Dia = 1,
                Mes = 1,
                Nombre = "Año Nuevo",
                IdTipo = 1
            };
            // Act: usamos reflexión para invocar el método privado ObtenerFestivo
            var metodo = servicio.GetType().GetMethod("ObtenerFestivo", BindingFlags.NonPublic | BindingFlags.Instance);
            var resultado = (FechaFestivo)metodo.Invoke(servicio, new object[] { 2025, festivo });

            // Assert: la fecha y el nombre deben coincidir con los esperados

            Assert.Equal(new DateTime(2025, 1, 1), resultado.Fecha);
            Assert.Equal("Año Nuevo", resultado.Nombre);

        }

        /// <summary>
        /// Verifica que un festivo tipo 2 (puente festivo) sea trasladado al lunes siguiente
        /// </summary>

        [Fact]
        public void ObtenerFestivo_Tipo2()
        {
            var servicio = CrearServicio();
            var festivo = new Festivo
            {
                Dia = 19,
                Mes = 3,
                Nombre = "San José",
                IdTipo = 2
            };
            // Act: invocamos ObtenerFestivo para calcular la fecha trasladada
            var metodo = servicio.GetType().GetMethod("ObtenerFestivo", BindingFlags.NonPublic | BindingFlags.Instance);
            var resultado = (FechaFestivo)metodo.Invoke(servicio, new object[] { 2025, festivo });

            // Assert: debe caer en lunes y ser posterior al 19 de marzo

            Assert.Equal(DayOfWeek.Monday, resultado.Fecha.DayOfWeek);
            Assert.True (resultado.Fecha > new DateTime(2025,3,19));


        }

        /// <summary>
        /// Verifica que un festivo tipo 4 (basado en Pascua y con traslado) caiga en lunes
        /// </summary>

        [Fact]
        public void ObtenerFestivo_Tipo4()
        {
            var servicio = CrearServicio();
            var festivo = new Festivo
            {
                DiasPascua = 68,//SAGRADO CORAZON 
                Nombre = "Sagrado Corazón",
                IdTipo = 4
            };

            // Act: se calcula la fecha del festivo relativo a Pascua y se traslada al lunes
            var metodo = servicio.GetType().GetMethod("ObtenerFestivo", BindingFlags.NonPublic | BindingFlags.Instance);
            var resultado = (FechaFestivo)metodo.Invoke(servicio, new object[] { 2025, festivo });

            // Assert: debe caer en lunes y tener el nombre correcto
            Assert.Equal(DayOfWeek.Monday, resultado.Fecha.DayOfWeek);
            Assert.Equal("Sagrado Corazón", resultado.Nombre);

        }



    }


}
