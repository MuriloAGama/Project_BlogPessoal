using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogPessoal.src.contextos;
using BlogPessoal.src.modelos;
using Microsoft.EntityFrameworkCore;

namespace BlogPessoal.src.repositorios.implementacoes
{
    /// <summary>
    /// <para>Resumo: Classe responsavel por implementar ITema</para>
    /// <para>Criado por: Murilo Gama</para>
    /// <para>Versão: 1.0</para>
    /// <para>Data: 12/05/2022</para>
    /// </summary>
    public class TemaRepositorio : ITema
    {
        #region Atributos
       
        private readonly BlogPessoalContexto _contexto;
        
        #endregion
            
        #region Construtores
		
        public TemaRepositorio(BlogPessoalContexto contexto)
        {
        	_contexto = contexto;
        }
        
        #endregion
     
        #region Métodos

        /// <summary>
        /// <para>Resumo: Método assíncrono para pegar todos temas</para>
        /// </summary>
        /// <return>Lista TemaModelo</return>
        public async Task<List<Tema>> PegarTodosTemasAsync() 
        {
            return await _contexto.Temas.ToListAsync();
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para pegar um tema pelo Id</para>
        /// </summary>
        /// <param name="id">Id do tema</param>
        /// <return>TemaModelo</return>
        /// <exception cref="Exception">Id não pode ser nulo</exception>
        public async Task<Tema> PegarTemaPeloIdAsync(int id)
        {
            if (!ExisteId(id)) throw new Exception("Id do tema não encontrado");

            return await _contexto.Temas.FirstOrDefaultAsync(t => t.Id == id);

            // funções auxiliares
            bool ExisteId(int id)
            {
                var auxiliar = _contexto.Temas.FirstOrDefault(u => u.Id == id);
                return auxiliar != null;
            }
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para pegar temas pela descrição</para>
        /// </summary>
        /// <param name="descricao">Descrição do tema</param>
        /// <return>Lista TemaModelo</return>
        public async Task<List<Tema>> PegarTemasPelaDescricaoAsync(string descricao)
        {
            return await _contexto.Temas
                            .Where(u => u.Descricao.Contains(descricao))
                            .ToListAsync();
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para salvar um novo tema</para>
        /// </summary>
        /// <param name="tema">Construtor para cadastrar tema</param>
        public async Task NovoTemaAsync(Tema tema)
        {
            await _contexto.Temas.AddAsync(new Tema
            {
                Descricao = tema.Descricao
            });

            await _contexto.SaveChangesAsync();
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para atualizar um tema</para>
        /// </summary>
        /// <param name="tema">Construtor para atualizar tema</param>
        public async Task AtualizarTemaAsync(Tema tema)  
        {
            var temaExistente = await PegarTemaPeloIdAsync(tema.Id);
            temaExistente.Descricao = tema.Descricao;

            _contexto.Temas.Update(temaExistente);
            await _contexto.SaveChangesAsync();
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para deletar um tema</para>
        /// </summary>
        /// <param name="id">Id do tema</param>
        public async Task DeletarTemaAsync(int id)
        {
            _contexto.Temas.Remove(await PegarTemaPeloIdAsync(id));
            await _contexto.SaveChangesAsync();
        }
            
        #endregion
    }
}