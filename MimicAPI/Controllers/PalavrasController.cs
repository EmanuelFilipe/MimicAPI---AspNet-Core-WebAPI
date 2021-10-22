using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.Models;
using Newtonsoft.Json;
using System.Linq;
using MimicAPI.Repositories.Contracts;
using AutoMapper;
using MimicAPI.Models.DTO;
using System.Collections.Generic;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper;

        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [Route("")]
        public IActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {
            var item = _repository.ObterPalavras(query);

            if (item.Count == 0) return NotFound();

            if (item.Paginacao != null)
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

            return Ok(item.ToList());
        }

        [HttpGet("{id}", Name = "ObterPalavra")]
        public IActionResult ObterPalavra(int id)
        {
            var palavra = _repository.Obter(id);
            if (palavra == null) return NotFound();

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links = new List<LinkDTO>();
            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("ObterPalavra", new { id }), "GET"));
            palavraDTO.Links.Add(new LinkDTO("update", Url.Link("AtualizarPalavra", new { id }), "PUT"));
            palavraDTO.Links.Add(new LinkDTO("delete", Url.Link("ExcluirPalavra", new { id }), "DELETE"));

            return Ok(palavraDTO);
        }

        [Route("")]
        [HttpPost]
        public IActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _repository.Cadastrar(palavra);
            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        [HttpPut("{id}", Name = "AtualizarPalavra")]
        public IActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            var model = _repository.Obter(id);
            if (model == null) return NotFound();

            palavra.Id = id;
            _repository.Atualizar(palavra);

            return Ok();
        }

        [HttpDelete("{id}", Name = "ExcluirPalavra")]
        public IActionResult Deletar(int id)
        {
            var palavra = _repository.Obter(id);
            if (palavra == null) return NotFound();

            _repository.Deletar(id);

            return NoContent();
        }
    }
}
