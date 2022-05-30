using BlogPessoal.src.modelos;
using BlogPessoal.src.repositorios;
using BlogPessoal.src.servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BlogPessoal.src.controladores
{
    [ApiController]
    [Route("api/Usuarios")]
    [Produces("application/json")]
    public class UsuarioControlador : ControllerBase
    {
        #region Atributos

        private readonly IUsuario _repositorio;
        private readonly IAutenticacao _servicos;

        #endregion

        #region Construtores

        public UsuarioControlador(IUsuario repositorio, IAutenticacao servico)
        {
            _repositorio = repositorio;
            _servicos = servico;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Pegar todos usuarios
        /// </summary>
        /// <returns>ActionResult</returns>
        /// <response code="200">Lista de usuarios</response>
        /// <response code="204">Lista vasia</response>
        [HttpGet("todos")]
        [Authorize]
        public async Task<ActionResult> PegarTodosUsuariosAsync()
        {
            var lista = await _repositorio.PegarTodosUsuariosAsync();

            if (lista.Count < 1) return NoContent();
            
            return Ok(lista);
        }

        /// <summary>
        /// Pegar usuario pelo Id
        /// </summary>
        /// <param name="idUsuario">Id do usuario</param>
        /// <returns>ActionResult</returns>
        /// <response code="200">Retorna o usuario</response>
        /// <response code="404">Id de usuario não existe</response>
        [HttpGet("id/{idUsuario}")]
        [Authorize(Roles = "NORMAL,ADMINISTRADOR")]
        public async Task<ActionResult> PegarUsuarioPeloIdAsync([FromRoute] int idUsuario)
        {
            try
            {
                return Ok(await _repositorio.PegarUsuarioPeloIdAsync(idUsuario));
            }
            catch (Exception ex)
            {
                return NotFound(new { Mensagem = ex.Message });
            }
            
        }

        /// <summary>
        /// Pegar usuario pelo Nome
        /// </summary>
        /// <param name="nomeUsuario">Nome do usuario</param>
        /// <returns>ActionResult</returns>
        /// <response code="200">Retorna o usuario</response>
        /// <response code="204">Nome não existe</response>
        [HttpGet]
        [Authorize(Roles = "NORMAL,ADMINISTRADOR")]
        public async Task<ActionResult> PegarUsuariosPeloNomeAsync([FromQuery] string nomeUsuario)
        {
            var usuarios = await _repositorio.PegarUsuariosPeloNomeAsync(nomeUsuario);

            if (usuarios.Count < 1) return NoContent();

             return Ok(usuarios);
        }

        /// <summary>
        /// Pegar usuario pelo Email
        /// </summary>
        /// <param name="emailUsuario">E-mail do usuario</param>
        /// <returns>ActionResult</returns>
        /// <response code="200">Retorna o usuario</response>
        /// <response code="404">Email não existente</response>
        [HttpGet("email/{emailUsuario}")]
        [Authorize(Roles = "NORMAL,ADMINISTRADOR")]
        public async Task<ActionResult> PegarUsuarioPeloEmailAsync([FromRoute] string emailUsuario)
        {
            var usuario = await _repositorio.PegarUsuarioPeloEmailAsync(emailUsuario);

            if (usuario == null) return NotFound(new { Mensagem = "Usuario não encontrado" });

            return Ok(usuario);
        }

        /// <summary>
        /// Criar novo Usuario
        /// </summary>
        /// <param name="usuario">Contrutor para criar usuario</param>
        /// <returns>ActionResult</returns>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/Usuarios/cadastrar
        ///     {
        ///        "nome": "Gustavo Boaz",
        ///        "email": "gustavo@domain.com",
        ///        "senha": "134652",
        ///        "foto": "URLFOTO",
        ///        "tipo": "NORMAL"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Retorna usuario criado</response>
        /// <response code="401">E-mail ja cadastrado</response>
        [HttpPost("cadastrar")]
        [AllowAnonymous]
        public async Task<ActionResult> NovoUsuarioAsync([FromBody] Usuario usuario)
        {
            try
            {
                await _servicos.CriarUsuarioSemDuplicarAsync(usuario);
                return Created($"api/Usuarios/email/{usuario.Email}", usuario);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Atualizar Usuario
        /// </summary>
        /// <param name="usuario">Construtor para atualizar usuario</param>
        /// <returns>ActionResult</returns>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     PUT /api/Usuarios
        ///     {
        ///        "id": 1,    
        ///        "nome": "Gustavo Boaz",
        ///        "senha": "134652",
        ///        "foto": "URLFOTO",
        ///        "tipo": "ADMINISTRADOR"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Retorna usuario atualizado</response>
        /// <response code="400">Erro na requisição</response>
        [HttpPut]
        [Authorize(Roles = "NORMAL,ADMINISTRADOR")]
        public async Task<ActionResult> AtualizarUsuarioAsync([FromBody] Usuario usuario)
        {
            usuario.Senha = _servicos.CodificarSenha(usuario.Senha);

            try
            {
                await _repositorio.AtualizarUsuarioAsync(usuario);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Pegar Autorização
        /// </summary>
        /// <param name="usuario">Construtor para logar usuario</param>
        /// <returns>ActionResult</returns>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/Usuarios/logar
        ///     {
        ///        "email": "gustavo@domain.com",
        ///        "senha": "134652"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Retorna usuario criado</response>
        /// <response code="401">E-mail ou senha invalido</response>
        [HttpPost("logar")]
        [AllowAnonymous]
        public async Task<ActionResult> LogarAsync([FromBody] Usuario usuario)
        {
            var auxiliar = await _repositorio.PegarUsuarioPeloEmailAsync(usuario.Email);

            if (auxiliar == null) return Unauthorized(new { Mensagem = "E-mail invalido" });

            if (auxiliar.Senha != _servicos.CodificarSenha(usuario.Senha)) return Unauthorized(new { Mensagem = "Senha invalida" });

            var token = "Bearer " + _servicos.GerarToken(auxiliar);

            return Ok(new { Usuario = auxiliar,  Token = token });
        }

        /// <summary>
        /// Deletar usuario pelo Id
        /// </summary>
        /// <param name="idUsuario">Id do usuario</param>
        /// <returns>ActionResult</returns>
        /// <response code="204">Usuario deletado</response>
        /// <response code="404">Id de usuario invalido</response>
        [HttpDelete("deletar/{idUsuario}")]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<ActionResult> DeletarUsuarioAsync([FromRoute] int idUsuario)
        {
            try
            {
                await _repositorio.DeletarUsuarioAsync(idUsuario);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { Mensagem = ex.Message });
            }
        }

        #endregion
    }
}