﻿using sistema_fichas.Business;
using sistema_fichas.Service.Core;
using sistema_fichas.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace sistema_fichas.WebApi.Controllers
{
    public class PedidoController : ApiController
    {
        IPedidoService _PedidoService;
        IPedidoDetalleService _PedidoDetalleService;
        IClienteService _ClienteService;

        public PedidoController(IPedidoService PedidoService, IClienteService ClienteService, IPedidoDetalleService PedidoDetalleService)
        {
            _PedidoService = PedidoService;
            _ClienteService = ClienteService;
            _PedidoDetalleService = PedidoDetalleService;
        }

        public IList<PedidoConDetalles> Get()
        {
            var pedidos = _PedidoService.GetAllByCriteria(null, null, true).ToList().Where(x => x.EstadoPedido.Estado == 5);
            IList<PedidoConDetalles> pedidosConDetalles = new List<PedidoConDetalles>();
            pedidosConDetalles.Clear();

            foreach (Pedido p in pedidos)
            {
                PedidoConDetalles pedidoConDetalle = new PedidoConDetalles();
                pedidoConDetalle.Pedido_ID = p.ID;
                pedidoConDetalle.Pedido_FechaInicio = p.FechaInicio.Value;

                pedidoConDetalle.Estado_ID = p.EstadoPedido_ID.Value;
                pedidoConDetalle.Estado_Codigo = p.EstadoPedido.Estado;
                pedidoConDetalle.Estado_Nombre = p.EstadoPedido.Nombre;

                pedidoConDetalle.Ejecutivo_ID = p.UserProfile_ID;
                pedidoConDetalle.Ejecutivo_Nombre = p.UserProfile.UserName;

                IList<PedidoDetalle> actividades = null;
                actividades = p.PedidosDetalle.Where(x => x.Tipo == TipoPedidoDetalle.Actividad.GetHashCode()).ToList();

                if (actividades.Count > 0)
                {
                    pedidoConDetalle.Actividades = new List<ActividadDetalleDTO>();
                    pedidoConDetalle.Actividades = actividades.Select(x => new ActividadDetalleDTO
                    {

                        Actividad_ID = Convert.ToInt32(x.ID),
                        Actividad_Cantidad = x.Cantidad.Value,
                        Actividad_Nombre = x.Catalogo.Nombre,
                        Actividad_Fecha = x.FechaInicio.Value,
                        Estado_Nombre = x.EstadoDetalle.Nombre,
                        Moneda_Alias = x.Moneda.Alias


                    }).ToList();

                }
                else
                {
                    pedidoConDetalle.Actividades = null;
                }
                pedidosConDetalles.Add(pedidoConDetalle);
            }

            return pedidosConDetalles.ToList();
        }

        // GET api/pedido/5
        public IList<ActividadDTO> Get(int id)
        {
            var actividades = _PedidoDetalleService.GetPedidosDetalleActividad(id).ToList().Where(x => x.Pedido.EstadoPedido.Estado == TipoEstadoPedido.Aprobado_Operaciones.GetHashCode() && x.EstadoDetalle.Estado == TipoEstadoDetalle.Activo.GetHashCode());
            IList<ActividadDTO> actividadesDTO = new List<ActividadDTO>();
            foreach (PedidoDetalle a in actividades)
            {
                ActividadDTO actividad = new ActividadDTO();
                actividad.ID = a.ID;
                actividad.Nombre = a.Catalogo.Nombre;
                actividadesDTO.Add(actividad);
            }
            return actividadesDTO;
        }

        // POST api/pedido
        public void Post([FromBody]string value)
        {
        }

        // PUT api/pedido/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/pedido/5
        public void Delete(int id)
        {
        }
    }
}
