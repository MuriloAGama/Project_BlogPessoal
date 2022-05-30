using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlogPessoal.src.modelos;
using BlogPessoal.src.repositorios;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlogPessoal.src.servicos.implementacoes
{
    /// <summary>
    /// <para>Resumo: Classe responsavel por implementar IAutenticacao</para>
    /// <para>Criado por: Gustavo Boaz</para>
    /// <para>Versão: 1.0</para>
    /// <para>Data: 12/05/2022</para>
    /// </summary>
    public class AutenticacaoServicos : IAutenticacao
    {
        #region Atributos

        private IUsuario _repositorio;
        public IConfiguration Configuracao { get; }

        #endregion

        #region Construtores

        public AutenticacaoServicos(IUsuario repositorio, IConfiguration configuration)
        {
            _repositorio = repositorio;
            Configuracao = configuration;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// <para>Resumo: Método responsavel por criptografar senha</para>
        /// </summary>
        /// <param name="senha">Senha a ser criptografada</param>
        /// <returns>string</returns>
        public string CodificarSenha(string senha)
        {
            var bytes = Encoding.UTF8.GetBytes(senha);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono responsavel por criar usuario sem duplicar no banco</para>
        /// </summary>
        /// <param name="usuario">Construtor para cadastrar usuario</param>
        public async Task CriarUsuarioSemDuplicarAsync(Usuario usuario)
        {
            var auxiliar = await _repositorio.PegarUsuarioPeloEmailAsync(usuario.Email);

            if (auxiliar != null) throw new Exception("Este email já está sendo utilizado");

            usuario.Senha = CodificarSenha(usuario.Senha);

            await _repositorio.NovoUsuarioAsync(usuario);
        }

        /// <summary>
        /// <para>Resumo: Método responsavel por gerar token JWT</para>
        /// </summary>
        /// <param name="usuario">Construtor de usuario que tenha parametros de e-mail e senha</param>
        /// <returns>string</returns>
        public string GerarToken(Usuario usuario)
        {
            var tokenManipulador = new JwtSecurityTokenHandler();
            var chave = Encoding.ASCII.GetBytes(Configuracao["Settings:Secret"]);
            var tokenDescricao = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Email, usuario.Email.ToString()),
                        new Claim(ClaimTypes.Role, usuario.Tipo.ToString())
                    }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(chave),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenManipulador.CreateToken(tokenDescricao);
            return tokenManipulador.WriteToken(token);
        }

        #endregion
    }
}