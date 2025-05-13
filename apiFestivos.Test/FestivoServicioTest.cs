using apiFestivos.Aplicacion.Servicios;
using apiFestivos.Core.Interfaces.Repositorios;
using apiFestivos.Dominio.Entidades;
using Moq;
using System;
using System.Collections.Generic;
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
}
