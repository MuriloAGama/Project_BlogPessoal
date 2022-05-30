using BlogPessoal.src.modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogPessoal.src.repositorios
{
    /// <summary>
    /// <para>Resumo: Responsavel por representar ações de CRUD de tema</para>
    /// <para>Criado por: Murilo Gama</para>
    /// <para>Versão: 1.0</para>
    /// <para>Data: 29/04/2022</para>
    /// </summary>
    public interface ITema
    {
        Task<List<Tema>> PegarTodosTemasAsync();
        Task<Tema> PegarTemaPeloIdAsync(int id);
        Task<List<Tema>> PegarTemasPelaDescricaoAsync(string descricao);
        Task NovoTemaAsync(Tema tema);
        Task AtualizarTemaAsync(Tema tema);
        Task DeletarTemaAsync(int id);
    }
}
