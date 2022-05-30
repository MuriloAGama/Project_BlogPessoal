using System;
using System.Threading.Tasks;
using BlogPessoal.src.contextos;
using BlogPessoal.src.modelos;
using BlogPessoal.src.repositorios;
using BlogPessoal.src.repositorios.implementacoes;
using BlogPessoal.src.servicos;
using BlogPessoal.src.servicos.implementacoes;
using BlogPessoal.src.utilidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogPessoalTeste.Testes.servicos
{
    [TestClass]
    public class AutenticacaoServicosTeste
    {
        private BlogPessoalContexto _contexto;
        private IUsuario _repositorio;
        private IAutenticacao _servicos;
        public IConfiguration Configuracao { get; }

        [TestMethod]
        public async Task CriarUsuarioDuplicadoRetornaErro()
        {
            // Definindo o contexto
            var opt = new DbContextOptionsBuilder<BlogPessoalContexto>()
                .UseInMemoryDatabase(databaseName: "db_blogpessoal")
                .Options;

            _contexto = new BlogPessoalContexto(opt);
            _repositorio = new UsuarioRepositorio(_contexto);
            _servicos = new AutenticacaoServicos(_repositorio, Configuracao);

            //GIVEN - Dado que registro um usuario no banco
            await _repositorio.NovoUsuarioAsync(new Usuario
            {
                Nome = "Gustavo Boaz",
                Email = "gustavo@email.com",
                Senha = "134652",
                Foto = "URLFOTO",
                Tipo = TipoUsuario.NORMAL
            });

            //WHEN - Quando tento criar um usuario com mesmo email
            await Assert.ThrowsExceptionAsync<Exception>( async () =>{
                await _servicos.CriarUsuarioSemDuplicarAsync(new Usuario
                {
                    Nome = "Gustavo Boaz",
                    Email = "gustavo@email.com",
                    Senha = "134652",
                    Foto = "URLFOTO",
                    Tipo = TipoUsuario.NORMAL
                });
            });
        }
        
    }
}