using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using Newtonsoft.Json;
using System.Linq;
using MimicAPI.V1.Repositories.Contracts;
using AutoMapper;
using MimicAPI.V1.Models.DTO;
using System.Collections.Generic;
using System;

namespace MimicAPI.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("1.1")]
    [Route("api/v{version:apiVersion}/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper;

        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [Route("", Name = "ObterTodas")]
        public IActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {
            var item = _repository.ObterPalavras(query);

            if (item.Results.Count == 0) return NotFound();
            PaginationList<PalavraDTO> lista = CriarLinksListPalavraDTO(query, item);

            return Ok(lista);
        }

        private PaginationList<PalavraDTO> CriarLinksListPalavraDTO(PalavraUrlQuery query, PaginationList<Palavra> item)
        {
            var lista = _mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDTO>>(item);

            foreach (var palavra in lista.Results)
            {
                palavra.Links.Add(new LinkDTO("self", Url.Link("ObterPalavra", new { palavra.Id }), "GET"));
            }

            lista.Links.Add(new LinkDTO("self", Url.Link("ObterTodas", query), "GET"));

            if (item.Paginacao != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

                if (query.PagNumero + 1 <= item.Paginacao.TotalPaginas)
                {
                    var queryString = new PalavraUrlQuery()
                    {
                        PagNumero = query.PagNumero + 1,
                        PagRegistro = query.PagRegistro,
                        Data = query.Data
                    };

                    lista.Links.Add(new LinkDTO("next", Url.Link("ObterTodas", queryString), "GET"));
                }

                if (query.PagNumero - 1 > 0)
                {
                    var queryString = new PalavraUrlQuery()
                    {
                        PagNumero = query.PagNumero - 1,
                        PagRegistro = query.PagRegistro,
                        Data = query.Data
                    };

                    lista.Links.Add(new LinkDTO("prev", Url.Link("ObterTodas", queryString), "GET"));
                }
            }

            return lista;
        }

        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpGet("{id}", Name = "ObterPalavra")]
        public IActionResult ObterPalavra(int id)
        {
            var palavra = _repository.Obter(id);
            if (palavra == null) return NotFound();

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("ObterPalavra", new { id }), "GET"));
            palavraDTO.Links.Add(new LinkDTO("update", Url.Link("AtualizarPalavra", new { id }), "PUT"));
            palavraDTO.Links.Add(new LinkDTO("delete", Url.Link("ExcluirPalavra", new { id }), "DELETE"));

            return Ok(palavraDTO);
        }

        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [Route("")]
        [HttpPost]
        public IActionResult Cadastrar([FromBody]Palavra palavra)
        {
            if (palavra == null) return BadRequest();

            if (!ModelState.IsValid) return new UnprocessableEntityObjectResult(ModelState);
            palavra.Ativo = true;
            palavra.Criado = DateTime.Now;
            _repository.Cadastrar(palavra);

            var palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "GET"));

            return Created($"/api/palavras/{palavra.Id}", palavraDTO);
        }

        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpPut("{id}", Name = "AtualizarPalavra")]
        public IActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            var model = _repository.Obter(id);
            if (model == null) return NotFound();

            palavra.Id = id;
            palavra.Ativo = model.Ativo;
            palavra.Criado = model.Criado;
            palavra.Atualizado = DateTime.Now;
            _repository.Atualizar(palavra);

            var palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "GET"));

            return Ok();
        }

        [MapToApiVersion("1.1")]
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
