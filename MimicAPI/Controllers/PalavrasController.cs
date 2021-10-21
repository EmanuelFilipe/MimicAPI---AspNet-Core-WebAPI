using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimicAPI.Database;
using MimicAPI.Models;
using MimicAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        public IActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {
            var item = _banco.Palavras.AsQueryable();

            if (query.Data.HasValue)
                item = item.Where(p => p.Criado > query.Data.Value || p.Atualizado > query.Data.Value);

            if (query.PagNumero.HasValue)
            {
                var quantidadeTotalRegistros = item.Count();
                item = item.Skip((query.PagNumero.Value - 1) * query.PagRegistro.Value).Take(query.PagRegistro.Value);

                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.PagNumero.Value;
                paginacao.RegistrosPorPagina = query.PagRegistro.Value;
                paginacao.TotalRegistros = quantidadeTotalRegistros;
                paginacao.TotalPaginas = (int)Math.Ceiling((double)quantidadeTotalRegistros / query.PagRegistro.Value);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));

                if (query.PagNumero > paginacao.TotalPaginas) return NotFound();
            }
            return Ok(item);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult ObterPalavra(int id)
        {
            var palavra = _banco.Palavras.Find(id);
            if (palavra == null) return NotFound();

            return Ok(palavra);
        }

        [Route("")]
        [HttpPost]
        public IActionResult Cadastrar([FromBody]Palavra model)
        {
            _banco.Palavras.Add(model);
            _banco.SaveChanges();

            return Created($"/api/palavras/{model.Id}", model);
        }

        [Route("{id}")]
        [HttpPut]
        public IActionResult Atualizar(int id, [FromBody]Palavra model)
        {
            var palavra = _banco.Palavras.AsNoTracking().FirstOrDefault(p => p.Id == id);
            if (palavra == null) return NotFound();

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
            if (palavra == null) return NotFound();

            palavra.Ativo = false;

            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return NoContent();
        }
    }
}
