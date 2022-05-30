using System;
using System.Threading.Tasks;
using BlogPessoal.src.modelos;
using BlogPessoal.src.repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogPessoal.src.controladores
{
    [ApiController]
    [Route("api/Postagens")]
    [Produces("application/json")]
    public class PostagemControlador : ControllerBase
    {
        #region Atributos

        private readonly IPostagem _repositorio;

        #endregion
        
        #region Construtores
        
        public PostagemControlador(IPostagem repositorio)
        {
            _repositorio = repositorio;
        }

        #endregion
        
        #region Métodos

        /// <summary>
        /// Pegar todas postagens
        /// </summary>
        /// <returns>ActionResult</returns>
        /// <response code="200">Lista de postagens</response>
        /// <response code="204">Lista vasia</response>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> PegarTodasPostagensAsync()
        {
            var lista = await _repositorio.PegarTodasPostagensAsync();

            if (lista.Count < 1) return NoContent();
            
            return Ok(lista);
        }

        /// <summary>
        /// Pegar postagem pelo Id
        /// </summary>
        /// <param name="idPostagem">Id da postagem</param>
        /// <returns>ActionResult</returns>
        /// <response code="200">Retorna a postagem</response>
        /// <response code="404">Id postagem não existente</response>
        [HttpGet("id/{idPostagem}")]
        [Authorize]
        public async Task<ActionResult> PegarPostagemPeloIdAsync([FromRoute] int idPostagem)
        {
            try
            {
                return Ok(await _repositorio.PegarPostagemPeloIdAsync(idPostagem));
            }
            catch (Exception ex)
            {
                return NotFound(new { Mensagem = ex.Message });
            }         
        }

        /// <summary>
        /// Pegar postagens por Pesquisa
        /// </summary>
        /// <param name="tituloPostagem">Titulo da postagem</param>
        /// <param name="descricaoTema">Descrição do tema</param>
        /// <param name="emailCriador">E-mail do criador</param>
        /// <returns>ActionResult</returns>
        /// <response code="200">Retorna postagens</response>
        /// <response code="204">Postagens não existe pra essa pesquisa</response>
        [HttpGet("pesquisa")]
        [Authorize]
        public async Task<ActionResult> PegarPostagensPorPesquisaAsync(
            [FromQuery] string tituloPostagem,
            [FromQuery] string descricaoTema,
            [FromQuery] string emailCriador)
        {
            var postagens = await _repositorio.PegarPostagensPorPesquisaAsync(tituloPostagem, descricaoTema, emailCriador);
            
            if (postagens.Count < 1) return NoContent();
            
            return Ok(postagens);
        }

        /// <summary>
        /// Criar nova Postagem
        /// </summary>
        /// <param name="postagem">Construtor para criar postagem</param>
        /// <returns>ActionResult</returns>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/Postagens
        ///     {  
        ///        "titulo": "Dotnet Core mudando o mundo", 
        ///        "descricao": "Uma ferramenta muito boa para desenvolver aplicações web",
        ///        "foto": "URLDAIMAGEM",
        ///        "criador": { "id": 1 },
        ///        "tema": { "id": 1 }
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Retorna postagem criada</response>
        /// <response code="400">Erro na requisição</response>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> NovaPostagemAsync([FromBody] Postagem postagem)
        {
            try
            {
                await _repositorio.NovaPostagemAsync(postagem);
                return Created($"api/Postagens", postagem);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Atualizar Postagem
        /// </summary>
        /// <param name="postagem">Construtor para atuallizar postagem</param>
        /// <returns>ActionResult</returns>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     PUT /api/Postagens
        ///     {
        ///        "id": 1,   
        ///        "titulo": "Dotnet Core mudando o mundo", 
        ///        "descricao": "Uma ferramenta muito boa para desenvolver aplicações web",
        ///        "foto": "URLDAIMAGEM",
        ///        "tema": { "id": 1 }
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Retorna postagem atualizada</response>
        /// <response code="400">Erro na requisição</response>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> AtualizarPostagemAsync([FromBody] Postagem postagem)
        {
            try
            {
                await _repositorio.AtualizarPostagemAsync(postagem);
                return Ok(postagem);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Deletar postagem pelo Id
        /// </summary>
        /// <param name="idPostagem">Id da postagem</param>
        /// <returns>ActionResult</returns>
        /// <response code="204">Postagem deletada</response>
        /// <response code="404">Id postagem não existente</response>
        [HttpDelete("deletar/{idPostagem}")]
        [Authorize]
        public async Task<ActionResult> DeletarPostagem([FromRoute] int idPostagem)
        {
            try
            {
                await _repositorio.DeletarPostagemAsync(idPostagem);
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