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
    public class FestivoServicioTest
    {
        [Fact]
        public async Task EsFestivo_RetornaTrue()
        {
            // Arrange
            var mockRepo = new Mock<IFestivoRepositorio>();
            mockRepo.Setup(r => r.ObtenerTodos()).ReturnsAsync(new List<Festivo>
            {
                new Festivo { IdTipo = 1, Nombre = "Año Nuevo", Dia = 1, Mes = 1 }
            });

            var servicio = new FestivoServicio(mockRepo.Object);
            var fecha = new DateTime(2025, 1, 1); // Año nuevo

            // Act
            var resultado = await servicio.EsFestivo(fecha);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task EsFestivo_RetornaFalse()
        {
            // Arrange
            var mockRepo = new Mock<IFestivoRepositorio>();
            mockRepo.Setup(r => r.ObtenerTodos()).ReturnsAsync(new List<Festivo>
            {
                new Festivo { IdTipo = 1, Nombre = "Año Nuevo", Dia = 1, Mes = 1 }
            });

            var servicio = new FestivoServicio(mockRepo.Object);
            var fecha = new DateTime(2025, 2, 15); // No festivo

            // Act
            var resultado = await servicio.EsFestivo(fecha);

            // Assert
            Assert.False(resultado);
        }
    }


    public class FestivoServicio_obternerFest_Test
    {
        private FestivoServicio CrearServicio()
        {
            var mockRepo = new Mock<IFestivoRepositorio>();
            return new FestivoServicio(mockRepo.Object);

        }

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
            // Act
            var metodo = servicio.GetType().GetMethod("ObtenerFestivo", BindingFlags.NonPublic | BindingFlags.Instance);
            var resultado = (FechaFestivo)metodo.Invoke(servicio, new object[] { 2025, festivo });

            // Assert

            Assert.Equal(new DateTime(2025, 1, 1), resultado.Fecha);
            Assert.Equal("Año Nuevo", resultado.Nombre);

        }

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

            var metodo = servicio.GetType().GetMethod("ObtenerFestivo", BindingFlags.NonPublic | BindingFlags.Instance);
            var resultado = (FechaFestivo)metodo.Invoke(servicio, new object[] { 2025, festivo });

            Assert.Equal(DayOfWeek.Monday, resultado.Fecha.DayOfWeek);
            Assert.True (resultado.Fecha > new DateTime(2025,3,19));


        }

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
            var metodo = servicio.GetType().GetMethod("ObtenerFestivo", BindingFlags.NonPublic | BindingFlags.Instance);
            var resultado = (FechaFestivo)metodo.Invoke(servicio, new object[] { 2025, festivo });

            // Assert
            Assert.Equal(DayOfWeek.Monday, resultado.Fecha.DayOfWeek);
            Assert.Equal("Sagrado Corazón", resultado.Nombre);

        }



    }


}
