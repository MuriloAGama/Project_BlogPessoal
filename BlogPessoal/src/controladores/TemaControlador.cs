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
    [Route("api/Temas")]
    [Produces("application/json")]
    public class TemaControlador : ControllerBase
    {
        #region Atributos
        
        private readonly ITema _repositorio;

        #endregion

        #region Construtores

        public TemaControlador(ITema repositorio)
        {
            _repositorio = repositorio;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Pegar todos temas
        /// </summary>
        /// <returns>ActionResult</returns>
        /// <response code="200">Lista de temas</response>
        /// <response code="204">Lista vasia</response>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> PegarTodosTemasAsync()
        {
            var lista = await _repositorio.PegarTodosTemasAsync();

            if (lista.Count < 1) return NoContent();
            
            return Ok(lista);
        }

        /// <summary>
        /// Pegar tema pelo Id
        /// </summary>
        /// <param name="idTema">Id do tema</param>
        /// <returns>ActionResult</returns>
        /// <response code="200">Retorna o tema</response>
        /// <response code="404">Tema não existente</response>
        [HttpGet("id/{idTema}")]
        [Authorize]
        public async Task<ActionResult> PegarTemaPeloIdAsync([FromRoute] int idTema)
        {
            try
            {
                return Ok(await _repositorio.PegarTemaPeloIdAsync(idTema));
            }
            catch (Exception ex)
            {
                return NotFound(new { Mensagem = ex.Message });
            }  
        }

        /// <summary>
        /// Pegar tema pela Descrição
        /// </summary>
        /// <param name="descricaoTema">Descrição do tema</param>
        /// <returns>ActionResult</returns>
        /// <response code="200">Retorna temas</response>
        /// <response code="204">Descrição não existe</response>
        [HttpGet("pesquisa")]
        [Authorize]
        public async Task<ActionResult> PegarTemasPelaDescricaoAsync([FromQuery] string descricaoTema)
        {
            var temas = await _repositorio.PegarTemasPelaDescricaoAsync(descricaoTema);

            if (temas.Count < 1) return NoContent();
            
            return Ok(temas);
        }

        /// <summary>
        /// Criar novo Tema
        /// </summary>
        /// <param name="tema">Construtor para criar tema</param>
        /// <returns>ActionResult</returns>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/Temas
        ///     {
        ///        "descricao": "CSHARP"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Retorna tema criado</response>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> NovoTemaAsync([FromBody] Tema tema)
        {
            await _repositorio.NovoTemaAsync(tema);
            
            return Created($"api/Temas", tema);
        }

        /// <summary>
        /// Atualizar Tema
        /// </summary>
        /// <param name="tema">Contrutor para alterar tema</param>
        /// <returns>ActionResult</returns>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     PUT /api/Temas
        ///     {
        ///        "id": 1,    
        ///        "descricao": "CSHARP"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Retorna tema atualizado</response>
        /// <response code="400">Erro na requisição</response>
        [HttpPut]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<ActionResult> AtualizarTema([FromBody] Tema tema)
        {
            try
            {
                await _repositorio.AtualizarTemaAsync(tema);
                return Ok(tema);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Deletar tema pelo Id
        /// </summary>
        /// <param name="idTema">Id do tema</param>
        /// <returns>ActionResult</returns>
        /// <response code="204">Tema deletado</response>
        /// <response code="404">Id tema não existe</response>
        [HttpDelete("deletar/{idTema}")]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<ActionResult> DeletarTema([FromRoute] int idTema)
        {
            try
            {
                await _repositorio.DeletarTemaAsync(idTema);
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