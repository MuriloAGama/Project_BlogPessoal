using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using BlogPessoal.src.utilidades;
using BlogPessoal.src.contextos;
using BlogPessoal.src.modelos;
using System.Threading.Tasks;

namespace BlogPessoalTeste.Testes.data
{
    [TestClass]
    public class BlogPessoalContextoTeste
    {
        private BlogPessoalContexto _contexto;

        [TestMethod]
        public async Task InserirNovoUsuarioNoBancoRetornarUsuario()
        {
            var opt = new DbContextOptionsBuilder<BlogPessoalContexto>()
                .UseInMemoryDatabase(databaseName: "db_blogpessoal")
                .Options;

            _contexto = new BlogPessoalContexto(opt);

            await _contexto.Usuarios.AddAsync(new Usuario
            {
                Nome = "Karol Boaz",
                Email = "karol@email.com",
                Senha = "134652",
                Foto = "AKITAOLINKDAFOTO",
                Tipo = TipoUsuario.NORMAL
            });

            await _contexto.SaveChangesAsync();

            Assert.IsNotNull(await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == "karol@email.com"));
        }
    }
}
