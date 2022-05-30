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
    /// <para>Resumo: Classe responsavel por implementar IUsuario</para>
    /// <para>Criado por: Murilo Gama</para>
    /// <para>Versão: 1.0</para>
    /// <para>Data: 12/05/2022</para>
    /// </summary>
    public class UsuarioRepositorio : IUsuario
    {
        #region Atributos

        private readonly BlogPessoalContexto _contexto;

        #endregion

        #region Construtores

        public UsuarioRepositorio(BlogPessoalContexto contexto)
        {
            _contexto = contexto;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// <para>Resumo: Método assíncrono para pegar todos usuarios</para>
        /// </summary>
        /// <return>Lista Usuarios</return>
        public async Task<List<Usuario>> PegarTodosUsuariosAsync() 
        {
            return await _contexto.Usuarios.ToListAsync();
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para pegar um usuario pelo Id</para>
        /// </summary>
        /// <param name="id">Id do usuario</param>
        /// <return>UsuarioModelo</return>
        /// <exception cref="Exception">Id não pode ser nulo</exception>
        public async Task<Usuario> PegarUsuarioPeloIdAsync(int id)
        {
            if (!ExisteId(id)) throw new Exception("Id de usuario não encontrado");

            return await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

            // funções auxiliares
            bool ExisteId(int id)
            {
                var auxiliar = _contexto.Usuarios.FirstOrDefault(u => u.Id == id);
                return auxiliar != null;
            }
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para pegar usuarios pelo nome</para>
        /// </summary>
        /// <param name="nome">Nome do usuario</param>
        /// <return>Lista UsuarioModelo</return>
        public async Task<List<Usuario>> PegarUsuariosPeloNomeAsync(string nome)
        {
            return await _contexto.Usuarios
                        .Where(u => u.Nome.Contains(nome))
                        .ToListAsync();
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para pegar um usuario pelo email</para>
        /// </summary>
        /// <param name="email">Email do usuario</param>
        /// <return>UsuarioModelo</return>
        public async Task<Usuario> PegarUsuarioPeloEmailAsync(string email)
        {
            return await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para salvar um novo usuario</para>
        /// </summary>
        /// <param name="usuario">Construtor para cadastrar usuario</param>
        public async Task NovoUsuarioAsync(Usuario usuario)
        {
            await _contexto.Usuarios.AddAsync(new Usuario
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                Senha = usuario.Senha,
                Foto = usuario.Foto,
                Tipo = usuario.Tipo
            });
            
            await _contexto.SaveChangesAsync();
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para atualizar um usuario</para>
        /// </summary>
        /// <param name="usuario">Construtor para atualizar usuario</param>
        public async Task AtualizarUsuarioAsync(Usuario usuario)
        {
            var usuarioExistente = await PegarUsuarioPeloIdAsync(usuario.Id);
            usuarioExistente.Nome = usuario.Nome;
            usuarioExistente.Senha = usuario.Senha;
            usuarioExistente.Foto = usuario.Foto;
            usuarioExistente.Tipo = usuario.Tipo;

            _contexto.Usuarios.Update(usuarioExistente);
            await _contexto.SaveChangesAsync();
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para deletar um usuario</para>
        /// </summary>
        /// <param name="id">Id do usuario</param>
        public async Task DeletarUsuarioAsync(int id)
        {
            _contexto.Usuarios.Remove(await PegarUsuarioPeloIdAsync(id));
            await _contexto.SaveChangesAsync();
        }

        #endregion
    }
}