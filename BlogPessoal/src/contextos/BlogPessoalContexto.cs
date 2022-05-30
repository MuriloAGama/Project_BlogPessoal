using BlogPessoal.src.modelos;
using Microsoft.EntityFrameworkCore;

namespace BlogPessoal.src.contextos
{
    /// <summary>
    /// <para>Resumo: Classe contexto, responsavel por carregar contexto e definir DbSets</para>
    /// <para>Criado por: Murilo Gama</para>
    /// <para>Versão: 1.0</para>
    /// <para>Data: 12/05/2022</para>
    /// </summary>
    public class BlogPessoalContexto : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tema> Temas { get; set; }
        public DbSet<Postagem> Postagens { get; set; }

        public BlogPessoalContexto(DbContextOptions<BlogPessoalContexto> opt) : base(opt)
        {

        }
    }
}
