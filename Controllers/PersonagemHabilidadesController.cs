using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;
//using FirstOrDefaultAsync;
//using System.Linq;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonagemHabilidadesController : ControllerBase//aula 11 parte 10
    {
        private readonly DataContext _context;
        //private object personagenshabilidades;
        public PersonagemHabilidadesController(DataContext context)// parte 11
        {
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddPersonagemHabilidadeAsync(PersonagemHabilidade novoPersonagemHabilidade)
        {
            try
            {
                Personagem personagem = await _context.TB_PERSONAGENS
                .Include(p => p.Arma)
                .Include(p => p.PersonagemHabilidades).ThenInclude(ps=>ps.Habilidade)
                .FirstOrDefaultAsync(p=> p.Id == novoPersonagemHabilidade.PersonagemId);

                if (personagem == null)
                   throw new System.Exception("Personagem não encontrado para o Id informado.");

                Habilidade habilidade = await _context.TB_HABILIDADES
                                     .FirstOrDefaultAsync(h=>h.Id == novoPersonagemHabilidade.HabilidadeId);

                if (habilidade == null)                     
                  throw new System.Exception("Habilidade não encontrada.");

                PersonagemHabilidade ph = new  PersonagemHabilidade();
                ph.Personagem = personagem;
                ph.Habilidade = habilidade;
                await _context.TB_PERSONAGENS_HABILIDADES.AddAsync(ph);
                //int linhasAfetadas = await _context.SaveChangesAsync();
                
                return Ok (ph);//(linhasAfetadas);
   
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
           /* [HttpGet("{id}")]
             public  async Task<IActionResult> GetSingle(int id)
            {
               try
               {
                Personagem p = await _context.TB_PERSONAGENS
                .Include(ar => ar.Arma)
                .Include(ph => ph.PersonagemHabilidades)
                  .ThenInclude(h => h.Habilidade)
                .FirstOrDefaultAsync(pBusca => pBusca.Id == id);

                return Ok(p);  
               }
               catch (System.Exception ex)
               {
                return BadRequest(ex.Message);
               }
            }

             
           /* {
                List<PersonagemHabilidade> listaFinal = personagenshabilidades.OrderBy(p => p.id).ToList();
                return Ok(listaFinal);
            }
            [HttpGet("GetHabilidades")]
            public IActionResult GetHabilidade()
            {
             
            }
            [HttpPost(" ")]
            public IActionResult GetRemoverHabilidade()
            {
                
            }*/
           
        }
    }
}