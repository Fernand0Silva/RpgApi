using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Para usar recursos do EF Core
using RpgApi.Data; // Ajustar conforme necessário para o contexto do banco de dados
using RpgApi.Models;

namespace RpgApi.Utils
{
    public class Criptografia : ControllerBase // Baseado em ControllerBase para usar IActionResult
    {
        private readonly DataContext _context; // Substitua por seu tipo de contexto

        // Construtor para injetar o contexto do banco de dados
        public Criptografia(DataContext context)
        {
            _context = context;
        }

        public static void CriarPasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerificarPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != hash[i])
                    {
                        return false;
                    }
                }
                return true;
                //return computedHash.SequenceEqual(hash); // Melhor prática para comparação
            }
        }

        [HttpPost("Autenticar")]
        public async Task<IActionResult> AutenticarUsuario(Usuario credenciais)
        {
            try
            {
                var usuario = await _context.TB_USUARIOS
                    .FirstOrDefaultAsync(x => x.Username.ToLower().Equals(credenciais.Username.ToLower()));

                if (usuario == null) // Verifica se o usuário existe
                {
                    throw new System.Exception("Usuário não encontrado.");
                }
                else if (!Criptografia
                       .VerificarPasswordHash(credenciais.PasswordString, usuario.PasswordHash, usuario.PasswordSalt))
                {
                    throw new System.Exception("Senha incorreta.");
                }       
                else 
                {
                    return Ok(usuario);
                }
                

               /* bool senhaCorreta = VerificarPasswordHash(
                    credenciais.PasswordString,
                    usuario.PasswordHash,
                    usuario.PasswordSalt
                );

                if (!senhaCorreta) // Se a senha estiver errada
                {
                    return BadRequest("Senha incorreta.");
                }

                return Ok(usuario); // Retorna o usuário se a autenticação for bem-sucedida */
            }
            catch (System.Exception ex) // Captura e trata exceções
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Arma novaArma) // Método para adicionar uma arma
        {
            try
            {
                if (novaArma.Dano == 0)
                {
                    throw new Exception("O dano da arma não pode ser 0.");
                }

                var personagem = await _context.TB_PERSONAGENS
                    .FirstOrDefaultAsync(p => p.Id == novaArma.PersonagemId);

                if (personagem == null)
                {
                    throw new Exception("Personagem não encontrado com o Id informado.");
                }

                // Adiciona a nova arma ao banco de dados
                await _context.TB_ARMAS.AddAsync(novaArma);
                await _context.SaveChangesAsync();

                return Ok(novaArma.Id); // Retorna o ID da arma recém-criada
            }
            catch (Exception ex) // Tratamento de exceções para o método Add
            {
                return BadRequest($"Erro ao adicionar arma: {ex.Message}");
            }
        }
    }
}
