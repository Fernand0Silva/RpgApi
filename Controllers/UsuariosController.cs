using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;
using RpgApi.Utils;
//using Microsoft.EntityFrameworkCore;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController: ControllerBase
    {
        private readonly DataContext _context;
        public UsuariosController (DataContext context)
        {
            _context = context;
         }
        
        private async Task<bool> UsuarioExistente(string username)
        {
            if (await _context.TB_USUARIOS.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarUsuario(Usuario user)
        {
            try{
                if(await UsuarioExistente(user.Username))
                throw new System.Exception("Nome de usuário já exixte");

                Criptografia.CriarPasswordHash(
                user.PasswordString, out byte[] hash, out byte[ ] salt);
                user.PasswordString = string.Empty;
                user.PasswordHash = hash;
                user.PasswordHash = salt;
                await _context.TB_USUARIOS.AddAsync(user); 
                await _context.SaveChangesAsync(); 
                
                return Ok(user.Id);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("AlterarSenha")]
public async Task<IActionResult> AlterarSenha(int usuarioId, string novaSenha)
{
    try
    {
        var usuario = await _context.TB_USUARIOS.FindAsync(usuarioId);

        if (usuario == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        // Criptografa a nova senha
        Criptografia.CriarPasswordHash(novaSenha, out byte[] newHash, out byte[] newSalt);
        usuario.PasswordHash = newHash;
        usuario.PasswordSalt = newSalt;

        await _context.SaveChangesAsync(); // Salva as alterações no banco

        return Ok("Senha alterada com sucesso.");
    }
    catch (Exception ex)
    {
        return BadRequest($"Erro ao alterar senha: {ex.Message}");
    }
}

[HttpGet("ListarTodos")]
public async Task<IActionResult> ListarTodosUsuarios()
{
    var usuarios = await _context.TB_USUARIOS.ToListAsync(); // Lista todos os usuários
    return Ok(usuarios); // Retorna a lista de usuários
}
[HttpPost("Autenticar")]
public async Task<IActionResult> AutenticarUsuario(Usuario credenciais)
{
    try
    {
        var usuario = await _context.TB_USUARIOS
            .FirstOrDefaultAsync(x => x.Username.ToLower() == credenciais.Username.ToLower());

        if (usuario == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        bool senhaCorreta = Criptografia.VerificarPasswordHash(
            credenciais.PasswordString,
            usuario.PasswordHash,
            usuario.PasswordSalt
        );

        if (!senhaCorreta)
        {
            return BadRequest("Senha incorreta.");
        }

        usuario.DataAcesso = DateTime.Now; // Atualiza a data de acesso
        await _context.SaveChangesAsync(); // Salva a data de acesso

        return Ok(usuario);
    }
    catch (Exception ex)
    {
        return BadRequest($"Erro ao autenticar: {ex.Message}");
    }
}

[HttpGet("{usuarioId}")] 
 public async Task<IActionResult> GetUsuario(int usuarioId) 
 { 
 try
 { 
 //List exigirá o using System.Collections.Generic
 Usuario usuario = await _context.TB_USUARIOS //Busca o usuário no banco através do Id
 .FirstOrDefaultAsync(x => x.Id == usuarioId); 
 return Ok(usuario); 
 } 
 catch (System.Exception ex) 
 { 
 return BadRequest(ex.Message); 
 } 
 }

 [HttpGet("GetByLogin/{login}")] 
 public async Task<IActionResult> GetUsuario(string login) 
 { 
 try
 { 
 //List exigirá o using System.Collections.Generic
 Usuario usuario = await _context.TB_USUARIOS //Busca o usuário no banco através do login
 .FirstOrDefaultAsync(x => x.Username.ToLower() == login.ToLower()); 
 return Ok(usuario); 
 } 
 catch (System.Exception ex) 
 { 
 return BadRequest(ex.Message); 
 } 
 } 

 [HttpPut("AtualizarLocalizacao")] 
 public async Task<IActionResult> AtualizarLocalizacao(Usuario u) 
 { 
 try
 { 
    Usuario usuario = await _context.TB_USUARIOS //Busca o usuário no banco através do Id
    .FirstOrDefaultAsync(x => x.Id == u.Id); 
    usuario.Latitude = u.Latitude; 
    usuario.Longitude = u.Longitude; 
    var attach = _context.Attach(usuario); 
    attach.Property(x => x.Id).IsModified = false; 
    attach.Property(x => x.Latitude).IsModified = true; 
    attach.Property(x => x.Longitude).IsModified = true; 
    int linhasAfetadas = 0;//await _context.SaveChangesAsync(); //Confirma a alteração no banco
    return Ok(linhasAfetadas); //Retorna as linhas afetadas (Geralmente sempre 1 linha msm)
 } 
 catch (System.Exception ex) 
 { 
 return BadRequest(ex.Message); 
 } 
 } 

 [HttpPut("AtualizarEmail")] 
 public async Task<IActionResult> AtualizarEmail(Usuario u) 
 { 
 try
 { 
 Usuario usuario = await _context.TB_USUARIOS //Busca o usuário no banco através do Id
 .FirstOrDefaultAsync(x => x.Id == u.Id); 
 usuario.Email = u.Email; 
 var attach = _context.Attach(usuario); 
 attach.Property(x => x.Id).IsModified = false; 
 attach.Property(x => x.Email).IsModified = true; 
 int linhasAfetadas = 0;//await _context.SaveChangesAsync(); //Confirma a alteração no banco
 return Ok(linhasAfetadas); //Retorna as linhas afetadas (Geralmente sempre 1 linha msm)
 } 
 catch (System.Exception ex) 
 { 
 return BadRequest(ex.Message); 
 } 
 }

 [HttpPut("AtualizarFoto")] 
 public async Task<IActionResult> AtualizarFoto(Usuario u) 
 { 
 try
 { 
    Usuario usuario = await _context.TB_USUARIOS 
    .FirstOrDefaultAsync(x => x.Id == u.Id); 
    usuario.Foto = u.Foto; 
    var attach = _context.Attach(usuario); 
    attach.Property(x => x.Id).IsModified = false; 
    attach.Property(x => x.Foto).IsModified = true; 
    int linhasAfetadas = 0;//await _context.SaveChangesAsync(); 
    return Ok(linhasAfetadas); 
 } 
 catch (System.Exception ex) 
 { 
 return BadRequest(ex.Message); 
 } 
 } 








 }
}