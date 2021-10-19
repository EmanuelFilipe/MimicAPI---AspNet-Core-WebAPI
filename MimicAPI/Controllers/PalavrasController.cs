using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimicAPI.Database;
using MimicAPI.Models;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly MimicContext _banco;
        public PalavrasController(MimicContext banco)
        {
            _banco = banco;
        }
        
        [Route("")]
        [HttpGet]
        public IActionResult ObterTodas()
        {
            return Ok(_banco.Palavras);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult ObterPalavra(int id)
        {
            return Ok(_banco.Palavras.Find(id));
        }

        [Route("")]
        [HttpPost]
        public IActionResult Cadastrar([FromBody]Palavra model)
        {
            _banco.Palavras.Add(model);
            _banco.SaveChanges();

            return Ok();
        }

        [Route("{id}")]
        [HttpPut]
        public IActionResult Atualizar(int id, [FromBody]Palavra model)
        {
            model.Id = id;
            _banco.Palavras.Update(model);
            _banco.SaveChanges();

            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        public IActionResult Deletar(int id)
        {
            var palavra = _banco.Palavras.Find(id);
            palavra.Ativo = false;

            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok();
        }
    }
}
