using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApiFipe.Models.Context
{
    public partial class FIPEContratosContext : DbContext
    {
        public FIPEContratosContext()
        {
        }

        public FIPEContratosContext(DbContextOptions<FIPEContratosContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<Cidade> Cidade { get; set; }
        public virtual DbSet<ClassificacaoEmpresa> ClassificacaoEmpresa { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<ContaCorrente> ContaCorrente { get; set; }
        public virtual DbSet<Contrato> Contrato { get; set; }
        public virtual DbSet<ContratoAditivo> ContratoAditivo { get; set; }
        public virtual DbSet<ContratoCliente> ContratoCliente { get; set; }
        public virtual DbSet<ContratoComentario> ContratoComentario { get; set; }
        public virtual DbSet<ContratoComentarioLido> ContratoComentarioLido { get; set; }
        public virtual DbSet<ContratoContatos> ContratoContatos { get; set; }
        public virtual DbSet<ContratoCoordenador> ContratoCoordenador { get; set; }
        public virtual DbSet<ContratoCronogramaFinanceiro> ContratoCronogramaFinanceiro { get; set; }
        public virtual DbSet<ContratoCronogramaFinanceiroHistorico> ContratoCronogramaFinanceiroHistorico { get; set; }
        public virtual DbSet<ContratoCronogramaFinanceiroTemporaria> ContratoCronogramaFinanceiroTemporaria { get; set; }
        public virtual DbSet<ContratoDoc> ContratoDoc { get; set; }
        public virtual DbSet<ContratoDocPrincipal> ContratoDocPrincipal { get; set; }
        public virtual DbSet<ContratoDocsAcompanhaNf> ContratoDocsAcompanhaNf { get; set; }
        public virtual DbSet<ContratoEntregavel> ContratoEntregavel { get; set; }
        public virtual DbSet<ContratoEntregavelHistórico> ContratoEntregavelHistórico { get; set; }
        public virtual DbSet<ContratoEntregavelTemporaria> ContratoEntregavelTemporaria { get; set; }
        public virtual DbSet<ContratoEquipeTecnica> ContratoEquipeTecnica { get; set; }
        public virtual DbSet<ContratoHistorico> ContratoHistorico { get; set; }
        public virtual DbSet<ContratoParcelaEntregavel> ContratoParcelaEntregavel { get; set; }
        public virtual DbSet<ContratoParcelaEntregavelTemporaria> ContratoParcelaEntregavelTemporaria { get; set; }
        public virtual DbSet<ContratoProrrogacao> ContratoProrrogacao { get; set; }
        public virtual DbSet<ContratoReajuste> ContratoReajuste { get; set; }
        public virtual DbSet<ContratoReajusteHistoricoCronogramaFinanceiro> ContratoReajusteHistoricoCronogramaFinanceiro { get; set; }
        public virtual DbSet<EmailConfigurado> EmailConfigurado { get; set; }
        public virtual DbSet<Entidade> Entidade { get; set; }
        public virtual DbSet<EsferaEmpresa> EsferaEmpresa { get; set; }
        public virtual DbSet<Estados> Estados { get; set; }
        public virtual DbSet<FormaPagamento> FormaPagamento { get; set; }
        public virtual DbSet<FormacaoProfissional> FormacaoProfissional { get; set; }
        public virtual DbSet<Fornecedor> Fornecedor { get; set; }
        public virtual DbSet<Frente> Frente { get; set; }
        public virtual DbSet<FrenteCoordenador> FrenteCoordenador { get; set; }
        public virtual DbSet<FundamentoContratacao> FundamentoContratacao { get; set; }
        public virtual DbSet<HistoricoPessoaFisica> HistoricoPessoaFisica { get; set; }
        public virtual DbSet<HistoricoPessoaJuridica> HistoricoPessoaJuridica { get; set; }
        public virtual DbSet<IndiceReajuste> IndiceReajuste { get; set; }
        public virtual DbSet<Oportunidade> Oportunidade { get; set; }
        public virtual DbSet<OportunidadeCliente> OportunidadeCliente { get; set; }
        public virtual DbSet<OportunidadeContato> OportunidadeContato { get; set; }
        public virtual DbSet<OportunidadeDocs> OportunidadeDocs { get; set; }
        public virtual DbSet<OportunidadeResponsavel> OportunidadeResponsavel { get; set; }
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<Parametro> Parametro { get; set; }
        public virtual DbSet<Perfil> Perfil { get; set; }
        public virtual DbSet<PerfilUsuario> PerfilUsuario { get; set; }
        public virtual DbSet<Pessoa> Pessoa { get; set; }
        public virtual DbSet<PessoaFisica> PessoaFisica { get; set; }
        public virtual DbSet<PessoaJuridica> PessoaJuridica { get; set; }
        public virtual DbSet<Proposta> Proposta { get; set; }
        public virtual DbSet<PropostaCliente> PropostaCliente { get; set; }
        public virtual DbSet<PropostaComentario> PropostaComentario { get; set; }
        public virtual DbSet<PropostaComentarioLido> PropostaComentarioLido { get; set; }
        public virtual DbSet<PropostaContato> PropostaContato { get; set; }
        public virtual DbSet<PropostaCoordenador> PropostaCoordenador { get; set; }
        public virtual DbSet<PropostaDocs> PropostaDocs { get; set; }
        public virtual DbSet<PropostaDocsPrincipais> PropostaDocsPrincipais { get; set; }
        public virtual DbSet<PropostaHistorico> PropostaHistorico { get; set; }
        public virtual DbSet<Situacao> Situacao { get; set; }
        public virtual DbSet<TaxaInstitucional> TaxaInstitucional { get; set; }
        public virtual DbSet<Tema> Tema { get; set; }
        public virtual DbSet<TipoAditivo> TipoAditivo { get; set; }
        public virtual DbSet<TipoAdministracao> TipoAdministracao { get; set; }
        public virtual DbSet<TipoApresentacaoRelatorio> TipoApresentacaoRelatorio { get; set; }
        public virtual DbSet<TipoCobranca> TipoCobranca { get; set; }
        public virtual DbSet<TipoContato> TipoContato { get; set; }
        public virtual DbSet<TipoContratacaoEquipeTecnica> TipoContratacaoEquipeTecnica { get; set; }
        public virtual DbSet<TipoCoordenacao> TipoCoordenacao { get; set; }
        public virtual DbSet<TipoDocsAcompanhaNf> TipoDocsAcompanhaNf { get; set; }
        public virtual DbSet<TipoDocumento> TipoDocumento { get; set; }
        public virtual DbSet<TipoEntidade> TipoEntidade { get; set; }
        public virtual DbSet<TipoEntregaDocumento> TipoEntregaDocumento { get; set; }
        public virtual DbSet<TipoOportunidade> TipoOportunidade { get; set; }
        public virtual DbSet<TipoPessoa> TipoPessoa { get; set; }
        public virtual DbSet<TipoProposta> TipoProposta { get; set; }
        public virtual DbSet<TipoVinculo> TipoVinculo { get; set; }
        public virtual DbSet<UnidadeDeTempo> UnidadeDeTempo { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<VinculoPessoaFisica> VinculoPessoaFisica { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public static string ConnectionString         { get; set; }
        public static bool   EnvironmentIsProduction  { get; set; }             //EGS 30.05.2020 Se é ambiente de producao ou homologacao
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                .UseSqlServer(ConnectionString, options => options.EnableRetryOnFailure(
                 maxRetryCount: 10,
                 maxRetryDelay: TimeSpan.FromSeconds(30),
                 errorNumbersToAdd: null));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Area>(entity =>
            {
                entity.HasKey(e => e.IdArea);

                entity.Property(e => e.DsArea)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Cidade>(entity =>
            {
                entity.HasKey(e => e.IdCidade);

                entity.Property(e => e.CdIbge).HasColumnName("CdIBGE");

                entity.Property(e => e.CdUf).HasColumnName("CdUF");

                entity.Property(e => e.NmCidade)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Uf)
                    .HasColumnName("UF")
                    .HasMaxLength(2)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ClassificacaoEmpresa>(entity =>
            {
                entity.HasKey(e => e.IdClassificacaoEmpresa);

                entity.Property(e => e.DsClassificacaoEmpresa)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente);

                entity.HasOne(d => d.IdPessoaNavigation)
                    .WithMany(p => p.Cliente)
                    .HasForeignKey(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cliente_Pessoa");
            });

            modelBuilder.Entity<ContaCorrente>(entity =>
            {
                entity.HasKey(e => e.IdContaCorrente);

                entity.Property(e => e.IdContaCorrente).HasColumnName("idContaCorrente");

                entity.Property(e => e.CdAgencia)
                    .IsRequired()
                    .HasColumnName("cdAgencia")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CdBanco)
                    .IsRequired()
                    .HasColumnName("cdBanco")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.IcPadrao).HasColumnName("icPadrao");

                entity.Property(e => e.NmBanco)
                    .IsRequired()
                    .HasColumnName("nmBanco")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NuConta)
                    .IsRequired()
                    .HasColumnName("nuConta")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NuContaEditado)
                    .IsRequired()
                    .HasColumnName("nuContaEditado")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Contrato>(entity =>
            {
                entity.HasKey(e => e.IdContrato);

                entity.HasIndex(e => e.IdProposta)
                    .HasName("UQ_ContratoIdProposta")
                    .IsUnique()
                    .HasFilter("([IdProposta] IS NOT NULL)");

                entity.Property(e => e.IdContrato).ValueGeneratedNever();

                entity.Property(e => e.CdIss)
                    .HasColumnName("CdISS")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsApelido)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.DsAssunto).IsUnicode(false);

                entity.Property(e => e.DsObjeto).IsUnicode(false);

                entity.Property(e => e.DsObservacao).IsUnicode(false);

                entity.Property(e => e.DsPrazoExecucao)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsPrazoPagamento)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsTextoCorpoNf)
                    .HasColumnName("DsTextoCorpoNF")
                    .IsUnicode(false);

                entity.Property(e => e.DtAssinatura).HasColumnType("date");

                entity.Property(e => e.DtCriacao).HasColumnType("datetime");

                entity.Property(e => e.DtFim).HasColumnType("date");

                entity.Property(e => e.DtFimExecucao).HasColumnType("date");

                entity.Property(e => e.DtInicio).HasColumnType("date");

                entity.Property(e => e.DtInicioExecucao).HasColumnType("date");

                entity.Property(e => e.DtProxReajuste).HasColumnType("date");

                entity.Property(e => e.DtRenovacao).HasColumnType("date");

                entity.Property(e => e.DtUltimaAlteracao).HasColumnType("datetime");

                entity.Property(e => e.IcFatAprovEntregavel)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.IcFatPedidoEmpenho)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.IdContaCorrente).HasColumnName("idContaCorrente");

                entity.Property(e => e.NuAgencia)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NuBanco)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NuCentroCusto)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NuConta)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NuContratoCliente)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NuContratoEdit)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NuProcessoCliente)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VlContrato).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlCustoProjeto)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.VlDiferenca)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.VlOutrosCustos)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.VlOverhead)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.VlTotalCustoProjeto)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.VlTotalEquipeTecnica)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.VlTotalTaxaInstitucional)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.IdAreaNavigation)
                    .WithMany(p => p.Contrato)
                    .HasForeignKey(d => d.IdArea)
                    .HasConstraintName("FK_Contrato_Area");

                entity.HasOne(d => d.IdContaCorrenteNavigation)
                    .WithMany(p => p.Contrato)
                    .HasForeignKey(d => d.IdContaCorrente)
                    .HasConstraintName("FK_Contrato_ContaCorrente");

                entity.HasOne(d => d.IdFormaPagamentoNavigation)
                    .WithMany(p => p.Contrato)
                    .HasForeignKey(d => d.IdFormaPagamento)
                    .HasConstraintName("FK_Contrato_FormaPagamento");

                entity.HasOne(d => d.IdFundamentoNavigation)
                    .WithMany(p => p.Contrato)
                    .HasForeignKey(d => d.IdFundamento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Contrato_FundamentoContratacao");

                entity.HasOne(d => d.IdIndiceReajusteNavigation)
                    .WithMany(p => p.Contrato)
                    .HasForeignKey(d => d.IdIndiceReajuste)
                    .HasConstraintName("FK_Contrato_IndiceReajuste");

                entity.HasOne(d => d.IdPropostaNavigation)
                    .WithOne(p => p.Contrato)
                    .HasForeignKey<Contrato>(d => d.IdProposta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Contrato_Proposta");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.ContratoIdSituacaoNavigation)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Contrato_Situacao");

                entity.HasOne(d => d.IdSituacaoEquipeTecnicaNavigation)
                    .WithMany(p => p.ContratoIdSituacaoEquipeTecnicaNavigation)
                    .HasForeignKey(d => d.IdSituacaoEquipeTecnica)
                    .HasConstraintName("FK_Contrato_SituacaoEquipeTecnica");

                entity.HasOne(d => d.IdTemaNavigation)
                    .WithMany(p => p.Contrato)
                    .HasForeignKey(d => d.IdTema)
                    .HasConstraintName("FK_Contrato_Tema");

                entity.HasOne(d => d.IdTipoApresentacaoRelatorioNavigation)
                    .WithMany(p => p.Contrato)
                    .HasForeignKey(d => d.IdTipoApresentacaoRelatorio)
                    .HasConstraintName("FK_Contrato_TipoApresentacaoRelatorio");

                entity.HasOne(d => d.IdTipoCobrancaNavigation)
                    .WithMany(p => p.Contrato)
                    .HasForeignKey(d => d.IdTipoCobranca)
                    .HasConstraintName("FK_Contrato_TipoCobranca");

                entity.HasOne(d => d.IdTipoEntregaDocumentoNavigation)
                    .WithMany(p => p.Contrato)
                    .HasForeignKey(d => d.IdTipoEntregaDocumento)
                    .HasConstraintName("FK_Contrato_TipoEntregaDocumento");
            });

            modelBuilder.Entity<ContratoAditivo>(entity =>
            {
                entity.HasKey(e => e.IdContratoAditivo);

                entity.Property(e => e.DsAditivo).IsUnicode(false);

                entity.Property(e => e.DtAplicacao).HasColumnType("datetime");

                entity.Property(e => e.DtCriacao).HasColumnType("datetime");

                entity.Property(e => e.DtFim).HasColumnType("date");

                entity.Property(e => e.DtFimAditivada).HasColumnType("date");

                entity.Property(e => e.DtIniExecucaoAditivo).HasColumnType("date");

                entity.Property(e => e.DtInicio).HasColumnType("date");

                entity.Property(e => e.VlAditivo).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlContrato).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlContratoAditivado).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoAditivo)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_ContratoAditivo_Contrato");

                entity.HasOne(d => d.IdPropostaNavigation)
                    .WithMany(p => p.ContratoAditivo)
                    .HasForeignKey(d => d.IdProposta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ContratoA__IdPro__3513BDEB");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.ContratoAditivo)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoAditivo_Situacao");

                entity.HasOne(d => d.IdTipoAditivoNavigation)
                    .WithMany(p => p.ContratoAditivo)
                    .HasForeignKey(d => d.IdTipoAditivo)
                    .HasConstraintName("FK_ContratoAditivo_TipoAditivo");
            });

            modelBuilder.Entity<ContratoCliente>(entity =>
            {
                entity.HasKey(e => e.IdContratoCliente);

                entity.Property(e => e.NmFantasia)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RazaoSocial)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.ContratoCliente)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoCliente_Cliente");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoCliente)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_ContratoCliente_Contrato");
            });

            modelBuilder.Entity<ContratoComentario>(entity =>
            {
                entity.HasKey(e => e.IdContratoComentario);

                entity.Property(e => e.DsComentario)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.DtComentario).HasColumnType("datetime");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoComentario)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_ContratoComentario_Contrato");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.ContratoComentario)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoComentario_Usuario");
            });

            modelBuilder.Entity<ContratoComentarioLido>(entity =>
            {
                entity.HasKey(e => e.IdContratoComentarioLido);
            });

            modelBuilder.Entity<ContratoContatos>(entity =>
            {
                entity.HasKey(e => e.IdContratoContato)
                    .HasName("PK_ContratoConttratos");

                entity.Property(e => e.CdEmail)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsEndereco)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.NmContato)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.NmDepartamento)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.NuCelular)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NuTelefone)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoContatos)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_ContratoContatos_Contrato");

                entity.HasOne(d => d.IdContratoClienteNavigation)
                    .WithMany(p => p.ContratoContatos)
                    .HasForeignKey(d => d.IdContratoCliente)
                    .HasConstraintName("FK_ContratoContatos_ContratoClientes");

                entity.HasOne(d => d.IdTipoContatoNavigation)
                    .WithMany(p => p.ContratoContatos)
                    .HasForeignKey(d => d.IdTipoContato)
                    .HasConstraintName("FK_ContratoContatos_TipoContato");
            });

            modelBuilder.Entity<ContratoCoordenador>(entity =>
            {
                entity.HasKey(e => e.IdContratoCoordenador);

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoCoordenador)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_ContratoCoordenador_Contrato");

                entity.HasOne(d => d.IdPessoaNavigation)
                    .WithMany(p => p.ContratoCoordenador)
                    .HasForeignKey(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoCoordenador_Pessoa");

                entity.HasOne(d => d.IdTipoCoordenacaoNavigation)
                    .WithMany(p => p.ContratoCoordenador)
                    .HasForeignKey(d => d.IdTipoCoordenacao)
                    .HasConstraintName("FK_ContratoCoordenador_TipoCoordenacao");
            });

            modelBuilder.Entity<ContratoCronogramaFinanceiro>(entity =>
            {
                entity.HasKey(e => e.IdContratoCronFinanceiro);

                entity.Property(e => e.CdIss)
                    .HasColumnName("CdISS")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CdParcela)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DsObservacao).IsUnicode(false);

                entity.Property(e => e.DsTextoCorpoNf)
                    .HasColumnName("DsTextoCorpoNF")
                    .IsUnicode(false);

                entity.Property(e => e.DtFaturamento).HasColumnType("datetime");

                entity.Property(e => e.DtNotaFiscal).HasColumnType("date");

                entity.Property(e => e.IcAtraso).HasDefaultValueSql("((0))");

                entity.Property(e => e.NuNotaFiscal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VlParcela).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiro)
                    .HasForeignKey(d => d.IdContrato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiro_Contrato");

                entity.HasOne(d => d.IdContratoClienteNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiro)
                    .HasForeignKey(d => d.IdContratoCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiro_ContratoCliente");

                entity.HasOne(d => d.IdFrenteNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiro)
                    .HasForeignKey(d => d.IdFrente)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiro_Frente");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiro)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiro_Situacao");
            });

            modelBuilder.Entity<ContratoCronogramaFinanceiroHistorico>(entity =>
            {
                entity.HasKey(e => e.IdContratoCrongramaHistorico)
                    .HasName("PK_ContratoCronoFinanceiroHistorico");

                entity.Property(e => e.IdContratoCrongramaHistorico).HasColumnName("idContratoCrongramaHistorico");

                entity.Property(e => e.CdIss)
                    .HasColumnName("CdISS")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CdParcela)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DsObservacao).IsUnicode(false);

                entity.Property(e => e.DsTextoCorpoNf)
                    .HasColumnName("DsTextoCorpoNF")
                    .IsUnicode(false);

                entity.Property(e => e.DtFaturamento).HasColumnType("datetime");

                entity.Property(e => e.DtNotaFiscal).HasColumnType("date");

                entity.Property(e => e.NuEntregaveis)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.NuNotaFiscal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VlParcela).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.IdContratoAditivoNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiroHistorico)
                    .HasForeignKey(d => d.IdContratoAditivo)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiroHistorico_ContratoAditivo");

                entity.HasOne(d => d.IdContratoClienteNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiroHistorico)
                    .HasForeignKey(d => d.IdContratoCliente)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiroHistorico_ContratoCliente");

                entity.HasOne(d => d.IdContratoReajusteNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiroHistorico)
                    .HasForeignKey(d => d.IdContratoReajuste)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiroHistorico_ContratoReajuste");

                entity.HasOne(d => d.IdFrenteNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiroHistorico)
                    .HasForeignKey(d => d.IdFrente)
                    .HasConstraintName("FK__ContratoC__IdFre__14C6E9EA");
            });

            modelBuilder.Entity<ContratoCronogramaFinanceiroTemporaria>(entity =>
            {
                entity.HasKey(e => e.IdContratoCronFinanceiro);

                entity.Property(e => e.CdIss)
                    .HasColumnName("CdISS")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CdParcela)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DsObservacao).IsUnicode(false);

                entity.Property(e => e.DsTextoCorpoNf)
                    .HasColumnName("DsTextoCorpoNF")
                    .IsUnicode(false);

                entity.Property(e => e.DtFaturamento).HasColumnType("datetime");

                entity.Property(e => e.DtNotaFiscal).HasColumnType("date");

                entity.Property(e => e.IcAtraso).HasDefaultValueSql("((0))");

                entity.Property(e => e.NuNotaFiscal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VlParcela).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiroTemporaria)
                    .HasForeignKey(d => d.IdContrato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiroTemporaria_Contrato");

                entity.HasOne(d => d.IdContratoClienteNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiroTemporaria)
                    .HasForeignKey(d => d.IdContratoCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiroTemporaria_ContratoCliente");

                entity.HasOne(d => d.IdFrenteNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiroTemporaria)
                    .HasForeignKey(d => d.IdFrente)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiroTemporaria_Frente");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.ContratoCronogramaFinanceiroTemporaria)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoCronogramaFinanceiroTemporaria_Situacao");
            });

            modelBuilder.Entity<ContratoDoc>(entity =>
            {
                entity.HasKey(e => e.IdContratoDoc);

                entity.HasIndex(e => e.DocFisicoId)
                    .HasName("UQ__Contrato__66E037B2B59EC160")
                    .IsUnique();

                entity.Property(e => e.DocFisico).IsRequired();

                entity.Property(e => e.DocFisicoId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DsDoc)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.DtDoc).HasColumnType("date");

                entity.Property(e => e.DtUpload).HasColumnType("date");

                entity.Property(e => e.NmCriador)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoDoc)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_ContratoDoc_Contrato");

                entity.HasOne(d => d.IdTipoDocNavigation)
                    .WithMany(p => p.ContratoDoc)
                    .HasForeignKey(d => d.IdTipoDoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoDoc_TipoDocumento");
            });

            modelBuilder.Entity<ContratoDocPrincipal>(entity =>
            {
                entity.HasKey(e => e.IdContratoDocPrincipal);

                entity.HasIndex(e => e.DocFisicoId)
                    .HasName("UQ__Contrato__66E037B2CA95962F")
                    .IsUnique();

                entity.Property(e => e.DocFisico).IsRequired();

                entity.Property(e => e.DocFisicoId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DsDoc)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DtUpLoad).HasColumnType("datetime");

                entity.Property(e => e.NmCriador)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NmDocumento)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoDocPrincipal)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_ContratoDocPrincipal_Contrato");

                entity.HasOne(d => d.IdTipoDocNavigation)
                    .WithMany(p => p.ContratoDocPrincipal)
                    .HasForeignKey(d => d.IdTipoDoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoDocPrincipal_TipoDoc");
            });

            modelBuilder.Entity<ContratoDocsAcompanhaNf>(entity =>
            {
                entity.HasKey(e => e.IdContratoDocsAcompanhaNf);

                entity.ToTable("ContratoDocsAcompanhaNF");

                entity.Property(e => e.IdContratoDocsAcompanhaNf).HasColumnName("idContratoDocsAcompanhaNF");

                entity.Property(e => e.IdTipoDocsAcompanhaNf).HasColumnName("IdTipoDocsAcompanhaNF");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoDocsAcompanhaNf)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_ContratoDocsAcompanhaNF_Contrato");

                entity.HasOne(d => d.IdTipoDocsAcompanhaNfNavigation)
                    .WithMany(p => p.ContratoDocsAcompanhaNf)
                    .HasForeignKey(d => d.IdTipoDocsAcompanhaNf)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoDocsAcompanhaNF_TipoDocsAcompanhaNF");
            });

            modelBuilder.Entity<ContratoEntregavel>(entity =>
            {
                entity.HasKey(e => e.IdContratoEntregavel)
                    .HasName("PK_Entregavel");

                entity.Property(e => e.DsProduto)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DtProduto).HasColumnType("date");

                entity.Property(e => e.IcAtraso).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoEntregavel)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_ContratoProduto_Contrato");

                entity.HasOne(d => d.IdContratoClienteNavigation)
                    .WithMany(p => p.ContratoEntregavel)
                    .HasForeignKey(d => d.IdContratoCliente)
                    .HasConstraintName("FK_ContratoEntregavel_ContratoCliente");

                entity.HasOne(d => d.IdFrenteNavigation)
                    .WithMany(p => p.ContratoEntregavel)
                    .HasForeignKey(d => d.IdFrente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoProduto_Frente");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.ContratoEntregavel)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoProduto_Situacao");
            });

            modelBuilder.Entity<ContratoEntregavelHistórico>(entity =>
            {
                entity.HasKey(e => e.IdContratoEntregavelHistorico);

                entity.Property(e => e.DsProduto)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DtProduto).HasColumnType("date");

                entity.HasOne(d => d.IdContratoAditivoNavigation)
                    .WithMany(p => p.ContratoEntregavelHistórico)
                    .HasForeignKey(d => d.IdContratoAditivo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoEntregavelHistórico_ContratoAditivo");

                entity.HasOne(d => d.IdContratoClienteNavigation)
                    .WithMany(p => p.ContratoEntregavelHistórico)
                    .HasForeignKey(d => d.IdContratoCliente)
                    .HasConstraintName("FK_ContratoEntregavelHistórico_ContratoCliente");
            });

            modelBuilder.Entity<ContratoEntregavelTemporaria>(entity =>
            {
                entity.HasKey(e => e.IdContratoEntregavel)
                    .HasName("PK_EntregavelTemporaria");

                entity.Property(e => e.DsProduto)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DtProduto).HasColumnType("date");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoEntregavelTemporaria)
                    .HasForeignKey(d => d.IdContrato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoEntregavelTemporaria_Contrato");

                entity.HasOne(d => d.IdContratoClienteNavigation)
                    .WithMany(p => p.ContratoEntregavelTemporaria)
                    .HasForeignKey(d => d.IdContratoCliente)
                    .HasConstraintName("FK_ContratoEntregavelTemporaria_ContratoCliente");

                entity.HasOne(d => d.IdFrenteNavigation)
                    .WithMany(p => p.ContratoEntregavelTemporaria)
                    .HasForeignKey(d => d.IdFrente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoEntregavelTemporaria_Frente");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.ContratoEntregavelTemporaria)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoEntregavelTemporaria_Situacao");
            });

            modelBuilder.Entity<ContratoEquipeTecnica>(entity =>
            {
                entity.HasKey(e => e.IdContratoEquipeTecnica);

                entity.Property(e => e.DsAtividadeDesempenhada)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VlCustoProjeto)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.VlTotalAreceber)
                    .HasColumnName("VlTotalAReceber")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoEquipeTecnica)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_ContratoEquipeTecnica_Contrato");

                entity.HasOne(d => d.IdFormacaoProfissionalNavigation)
                    .WithMany(p => p.ContratoEquipeTecnica)
                    .HasForeignKey(d => d.IdFormacaoProfissional)
                    .HasConstraintName("FK_ContratoEquipeTecnica_FormacaoProfissional");

                entity.HasOne(d => d.IdPessoaFisicaNavigation)
                    .WithMany(p => p.ContratoEquipeTecnica)
                    .HasForeignKey(d => d.IdPessoaFisica)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoEquipeTecnica_PessoaFisica");

                entity.HasOne(d => d.IdPessoaJuridicaNavigation)
                    .WithMany(p => p.ContratoEquipeTecnica)
                    .HasForeignKey(d => d.IdPessoaJuridica)
                    .HasConstraintName("FK_ContratoEquipeTecnica_PessoaJuridica");

                entity.HasOne(d => d.IdTaxaInstitucionalNavigation)
                    .WithMany(p => p.ContratoEquipeTecnica)
                    .HasForeignKey(d => d.IdTaxaInstitucional)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoEquipeTecnica_TaxaInstitucional");

                entity.HasOne(d => d.IdTipoContratacaoNavigation)
                    .WithMany(p => p.ContratoEquipeTecnica)
                    .HasForeignKey(d => d.IdTipoContratacao)
                    .HasConstraintName("FK_ContratoEquipeTecnica_TipoContratacaoEquipeTecnica");
            });

            modelBuilder.Entity<ContratoHistorico>(entity =>
            {
                entity.HasKey(e => e.IdContratoHistorico)
                    .HasName("PK_ContraHistorico");

                entity.Property(e => e.IdContratoHistorico).HasColumnName("idContratoHistorico");

                entity.Property(e => e.DtFim).HasColumnType("datetime");

                entity.Property(e => e.DtInicio).HasColumnType("datetime");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoHistorico)
                    .HasForeignKey(d => d.IdContrato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoHistorico_Contrato");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.ContratoHistorico)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoHistorico_Situacao");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.ContratoHistorico)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoHistorico_Usuario");
            });

            modelBuilder.Entity<ContratoParcelaEntregavel>(entity =>
            {
                entity.HasKey(e => e.IdContratoParcelaEntregavel);

                entity.HasOne(d => d.IdEntregavelNavigation)
                    .WithMany(p => p.ContratoParcelaEntregavel)
                    .HasForeignKey(d => d.IdEntregavel)
                    .HasConstraintName("FK_ContratoParcelaEntregavel_Entregavel");

                entity.HasOne(d => d.IdParcelaNavigation)
                    .WithMany(p => p.ContratoParcelaEntregavel)
                    .HasForeignKey(d => d.IdParcela)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoParcelaEntregavel_Parcela");
            });

            modelBuilder.Entity<ContratoParcelaEntregavelTemporaria>(entity =>
            {
                entity.HasKey(e => e.IdContratoParcelaEntregavel);

                entity.HasOne(d => d.IdEntregavelNavigation)
                    .WithMany(p => p.ContratoParcelaEntregavelTemporaria)
                    .HasForeignKey(d => d.IdEntregavel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoParcelaEntregavelTemporaria_ContratoEntregavelTemporaria");

                entity.HasOne(d => d.IdParcelaNavigation)
                    .WithMany(p => p.ContratoParcelaEntregavelTemporaria)
                    .HasForeignKey(d => d.IdParcela)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoParcelaEntregavelTemporaria_ContratoCronogramaFinanceiroTemporaria");
            });

            modelBuilder.Entity<ContratoProrrogacao>(entity =>
            {
                entity.HasKey(e => e.IdContratoRenovacao);

                entity.Property(e => e.DsPrazoExecucao)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DtAplicacao).HasColumnType("datetime");

                entity.Property(e => e.DtFimExecucaoRenovacao).HasColumnType("date");

                entity.Property(e => e.DtFimVigenciaAtual).HasColumnType("date");

                entity.Property(e => e.DtFimVigenciaRenovacao).HasColumnType("date");

                entity.Property(e => e.DtInicioVigencia).HasColumnType("date");

                entity.Property(e => e.PcReajuste)
                    .HasColumnName("pcReajuste")
                    .HasColumnType("decimal(7, 4)");

                entity.Property(e => e.VlContratoAntesRenovacao)
                    .HasColumnName("vlContratoAntesRenovacao")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlContratoRenovado)
                    .HasColumnName("vlContratoRenovado")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlReajusteAcumulado)
                    .HasColumnName("vlReajusteAcumulado")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlReajustePeriodo)
                    .HasColumnName("vlReajustePeriodo")
                    .HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoProrrogacao)
                    .HasForeignKey(d => d.IdContrato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoProrrogacao_Contrato");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.ContratoProrrogacao)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoProrrogacao_Situacao");
            });

            modelBuilder.Entity<ContratoReajuste>(entity =>
            {
                entity.HasKey(e => e.IdContratoReajuste);

                entity.Property(e => e.DtAplicacao).HasColumnType("datetime");

                entity.Property(e => e.DtProxReajuste).HasColumnType("date");

                entity.Property(e => e.DtReajuste).HasColumnType("date");

                entity.Property(e => e.PcReajuste).HasColumnType("decimal(7, 4)");

                entity.Property(e => e.VlContratoAntesReajuste).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlContratoReajustado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlReajuste).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlReajusteAcumulado).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.ContratoReajuste)
                    .HasForeignKey(d => d.IdContrato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoReajuste_Contrato");

                entity.HasOne(d => d.IdIndiceReajusteNavigation)
                    .WithMany(p => p.ContratoReajuste)
                    .HasForeignKey(d => d.IdIndiceReajuste)
                    .HasConstraintName("FK_ContratoReajuste_IndiceReajuste");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.ContratoReajuste)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContratoReajuste_Situacao");
            });

            modelBuilder.Entity<ContratoReajusteHistoricoCronogramaFinanceiro>(entity =>
            {
                entity.HasKey(e => e.IdHistoricoCronogramaFinanceiro);

                entity.Property(e => e.IdHistoricoCronogramaFinanceiro).HasColumnName("idHistoricoCronogramaFinanceiro");

                entity.Property(e => e.CdIss)
                    .HasColumnName("CdISS")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CdParcela)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DsTextoCorpoNf)
                    .HasColumnName("DsTextoCorpoNF")
                    .IsUnicode(false);

                entity.Property(e => e.DtFaturamento).HasColumnType("date");

                entity.Property(e => e.DtNotaFiscal).HasColumnType("date");

                entity.Property(e => e.IdContratoReajuste).HasColumnName("idContratoReajuste");

                entity.Property(e => e.NuNotaFiscal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VlParcela).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.IdContratoReajusteNavigation)
                    .WithMany(p => p.ContratoReajusteHistoricoCronogramaFinanceiro)
                    .HasForeignKey(d => d.IdContratoReajuste)
                    .HasConstraintName("FK_ContratoReajusteHistoricoCronogramaFinanceiro_ContratoReajuste");
            });

            modelBuilder.Entity<EmailConfigurado>(entity =>
            {
                entity.HasKey(e => e.IdEmail);

                entity.Property(e => e.DsPerfil)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DsTexto)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.DsTitulo)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Entidade>(entity =>
            {
                entity.HasKey(e => e.IdEntidade);

                entity.Property(e => e.DsEntidade)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdTipoEntidadeNavigation)
                    .WithMany(p => p.Entidade)
                    .HasForeignKey(d => d.IdTipoEntidade)
                    .HasConstraintName("FK_Entidade_TipoEntidade");
            });

            modelBuilder.Entity<EsferaEmpresa>(entity =>
            {
                entity.HasKey(e => e.IdEsferaEmpresa);

                entity.Property(e => e.DsEsferaEmpresa)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdClassificacaoEmpresaNavigation)
                    .WithMany(p => p.EsferaEmpresa)
                    .HasForeignKey(d => d.IdClassificacaoEmpresa)
                    .HasConstraintName("FK_EsferaEmpresa_ClassificacaoEmpresa");
            });

            modelBuilder.Entity<Estados>(entity =>
            {
                entity.HasKey(e => e.IdEstado);

                entity.Property(e => e.NmEstado)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Uf)
                    .IsRequired()
                    .HasColumnName("UF")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FormaPagamento>(entity =>
            {
                entity.HasKey(e => e.IdFormaPagamento);

                entity.Property(e => e.DsFormaPagamento)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FormacaoProfissional>(entity =>
            {
                entity.HasKey(e => e.IdFormacaoProfissional);

                entity.Property(e => e.DsFormacaoProfissional)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Fornecedor>(entity =>
            {
                entity.HasKey(e => e.IdFornecedor);

                entity.HasIndex(e => e.IdFornecedor)
                    .HasName("IXFK_Fornecedor_PessoaJuridica_02");

                entity.HasOne(d => d.IdPessoaNavigation)
                    .WithMany(p => p.Fornecedor)
                    .HasForeignKey(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Fornecedor_Pessoa");
            });

            modelBuilder.Entity<Frente>(entity =>
            {
                entity.HasKey(e => e.IdFrente);

                entity.Property(e => e.CdFrenteTexto)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NmFrente)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.Frente)
                    .HasForeignKey(d => d.IdContrato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Frente_Contrato");
            });

            modelBuilder.Entity<FrenteCoordenador>(entity =>
            {
                entity.HasKey(e => e.IdFrenteCoordenador);

                entity.HasOne(d => d.IdFrenteNavigation)
                    .WithMany(p => p.FrenteCoordenador)
                    .HasForeignKey(d => d.IdFrente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FrenteCoordenador_Frente");

                entity.HasOne(d => d.IdPessoaFisicaNavigation)
                    .WithMany(p => p.FrenteCoordenador)
                    .HasForeignKey(d => d.IdPessoaFisica)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FrenteCoordenador_PessoaFisica");
            });

            modelBuilder.Entity<FundamentoContratacao>(entity =>
            {
                entity.HasKey(e => e.IdFundamento)
                    .HasName("PK_Fundamento");

                entity.Property(e => e.DsFundamento)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HistoricoPessoaFisica>(entity =>
            {
                entity.HasKey(e => e.IdHstPessoaFisica);

                entity.Property(e => e.CdCvLattes).IsUnicode(false);

                entity.Property(e => e.CdEmail)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CdLinkedIn).IsUnicode(false);

                entity.Property(e => e.CdSexo)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DsComplemento)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsEndereco)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DtAlteracao).HasColumnType("datetime");

                entity.Property(e => e.DtNascimento).HasColumnType("datetime");

                entity.Property(e => e.NmBairro)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NmPessoa)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NuCelular)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NuCep)
                    .HasColumnName("NuCEP")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.NuCpf)
                    .HasColumnName("NuCPF")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NuEndereco)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NuTelefoneComercial)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NuTelefoneFixo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SgUf)
                    .HasColumnName("SgUF")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdPessoaFisicaNavigation)
                    .WithMany(p => p.HistoricoPessoaFisica)
                    .HasForeignKey(d => d.IdPessoaFisica)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HistoricoPessoaFisica_PessoaFisica");

                entity.HasOne(d => d.IdTipoVinculoNavigation)
                    .WithMany(p => p.HistoricoPessoaFisica)
                    .HasForeignKey(d => d.IdTipoVinculo)
                    .HasConstraintName("FK_HistoricoPessoaFisica_TipoVinculo");
            });

            modelBuilder.Entity<HistoricoPessoaJuridica>(entity =>
            {
                entity.HasKey(e => e.IdHistPessoaJuridica);

                entity.Property(e => e.Cep)
                    .HasColumnName("CEP")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Cnpj)
                    .HasColumnName("CNPJ")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Complemento)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsInternacional)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DtAlteracao).HasColumnType("datetime");

                entity.Property(e => e.Endereco)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NmBairro)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NmCv)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NmFantasia)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.NuEndereco)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.RazaoSocial)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Uf)
                    .HasColumnName("UF")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCidadeNavigation)
                    .WithMany(p => p.HistoricoPessoaJuridica)
                    .HasForeignKey(d => d.IdCidade)
                    .HasConstraintName("FK_HistoricoPessoaJuridica_Cidade");

                entity.HasOne(d => d.IdClassificacaoEmpresaNavigation)
                    .WithMany(p => p.HistoricoPessoaJuridica)
                    .HasForeignKey(d => d.IdClassificacaoEmpresa)
                    .HasConstraintName("FK_HistoricoPessoaJuridica_ClassificacaoEmpresa");

                entity.HasOne(d => d.IdEsferaEmpresaNavigation)
                    .WithMany(p => p.HistoricoPessoaJuridica)
                    .HasForeignKey(d => d.IdEsferaEmpresa)
                    .HasConstraintName("FK_HistoricoPessoaJuridica_EsferaEmpresa");

                entity.HasOne(d => d.IdTipoAdministracaoNavigation)
                    .WithMany(p => p.HistoricoPessoaJuridica)
                    .HasForeignKey(d => d.IdTipoAdministracao)
                    .HasConstraintName("FK_HistoricoPessoaJuridica_TipoAdministracao");

                entity.HasOne(d => d.IdUsuarioAlteracaoNavigation)
                    .WithMany(p => p.HistoricoPessoaJuridica)
                    .HasForeignKey(d => d.IdUsuarioAlteracao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HistoricoPessoaJuridica_Usuario");
            });

            modelBuilder.Entity<IndiceReajuste>(entity =>
            {
                entity.HasKey(e => e.IdIndiceReajuste);

                entity.Property(e => e.DsIndiceReajuste)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            /* ===========================================================================================
            *  Edinaldo FIPE
            *  Agosto/2020 
            *  Insere o ERRO na tabela de LOG
            ===========================================================================================*/
            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.IdLog);

                entity.Property(e => e.DtLog)
                    .IsRequired();

                entity.Property(e => e.IdUsuario)
                    .IsRequired();

                entity.Property(e => e.DsMensagem)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });


            modelBuilder.Entity<Oportunidade>(entity =>
            {
                entity.HasKey(e => e.IdOportunidade);

                entity.Property(e => e.IdOportunidade).ValueGeneratedNever();

                entity.Property(e => e.DsAssunto).IsUnicode(false);

                entity.Property(e => e.DsCoordenacao)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DsObservacao).IsUnicode(false);

                entity.Property(e => e.DtCriacao).HasColumnType("date");

                entity.Property(e => e.DtLimiteEntregaProposta).HasColumnType("date");

                entity.Property(e => e.DtUltimaAlteracao).HasColumnType("datetime");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.Oportunidade)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Oportunidade_Situacao");

                entity.HasOne(d => d.IdTipoOportunidadeNavigation)
                    .WithMany(p => p.Oportunidade)
                    .HasForeignKey(d => d.IdTipoOportunidade)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Oportunidade_GrauOportunidade");
            });

            modelBuilder.Entity<OportunidadeCliente>(entity =>
            {
                entity.HasKey(e => e.IdOportunidadeCliente)
                    .HasName("PK_Table1");

                entity.Property(e => e.NmFantasia)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RazaoSocial)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.OportunidadeCliente)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK_OportunidadeCliente_Cliente");

                entity.HasOne(d => d.IdOportunidadeNavigation)
                    .WithMany(p => p.OportunidadeCliente)
                    .HasForeignKey(d => d.IdOportunidade)
                    .HasConstraintName("FK_OportunidadeCliente_Oportunidade");
            });

            modelBuilder.Entity<OportunidadeContato>(entity =>
            {
                entity.HasKey(e => e.IdOportunidadeContato);

                entity.Property(e => e.CdEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NmContato)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NmDepartamento)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.NuCelular)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NuTelefone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdTipoContatoNavigation)
                    .WithMany(p => p.OportunidadeContato)
                    .HasForeignKey(d => d.IdTipoContato)
                    .HasConstraintName("FK_OportunidadeContato_OportunidadeContato");
            });

            modelBuilder.Entity<OportunidadeDocs>(entity =>
            {
                entity.HasKey(e => e.IdOportunidadeDocs);

                entity.HasIndex(e => e.DocFisicoId)
                    .HasName("UQ__Oportuni__66E037B26D1C5D0A")
                    .IsUnique();

                entity.Property(e => e.DocFisico).IsRequired();

                entity.Property(e => e.DocFisicoId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DtUpLoad).HasColumnType("date");

                entity.Property(e => e.NmCriador)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NmDocumento)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.IdOportunidadeNavigation)
                    .WithMany(p => p.OportunidadeDocs)
                    .HasForeignKey(d => d.IdOportunidade)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OportunidadeDocs_Oportunidade");

                entity.HasOne(d => d.IdTipoDocumentoNavigation)
                    .WithMany(p => p.OportunidadeDocs)
                    .HasForeignKey(d => d.IdTipoDocumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OportunidadeDocs_TipoDocumento");
            });

            modelBuilder.Entity<OportunidadeResponsavel>(entity =>
            {
                entity.HasKey(e => e.IdOportunidadeResponsavel);

                entity.HasOne(d => d.IdOportunidadeNavigation)
                    .WithMany(p => p.OportunidadeResponsavel)
                    .HasForeignKey(d => d.IdOportunidade)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OportunidadeResponsavel_Oportunidade");

                entity.HasOne(d => d.IdPessoaFisicaNavigation)
                    .WithMany(p => p.OportunidadeResponsavel)
                    .HasForeignKey(d => d.IdPessoaFisica)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OportunidadeResponsavel_PessoaFisica");
            });

            modelBuilder.Entity<Pais>(entity =>
            {
                entity.HasKey(e => e.IdPais)
                    .HasName("PK__Paises__FC850A7B5E0BF8B6");

                entity.Property(e => e.IdPais).ValueGeneratedNever();

                entity.Property(e => e.Iso)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Iso3)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Parametro>(entity =>
            {
                entity.HasKey(e => e.IdParametro);

                entity.Property(e => e.DsPrazoPagto)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsTextoCorpoNf)
                    .HasColumnName("DsTextoCorpoNF")
                    .IsUnicode(false);

                entity.Property(e => e.EmailsNotificacao).IsUnicode(false);

                entity.Property(e => e.NuAgencia)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NuBanco)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NuConta)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NuPercentualOverhead).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.HasKey(e => e.IdPerfil);

                entity.Property(e => e.IdPerfil).ValueGeneratedNever();

                entity.Property(e => e.DsPerfil)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PerfilUsuario>(entity =>
            {
                entity.HasKey(e => e.IdPerfilUsuario);

                entity.HasOne(d => d.IdPerfilNavigation)
                    .WithMany(p => p.PerfilUsuario)
                    .HasForeignKey(d => d.IdPerfil)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PerfilUsuario_Perfil");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.PerfilUsuario)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PerfilUsuario_Usuario");
            });

            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.HasKey(e => e.IdPessoa);

                entity.HasOne(d => d.IdPessoaFisicaNavigation)
                    .WithMany(p => p.Pessoa)
                    .HasForeignKey(d => d.IdPessoaFisica)
                    .HasConstraintName("FK_Pessoa_PessoaFisica");

                entity.HasOne(d => d.IdPessoaJuridicaNavigation)
                    .WithMany(p => p.Pessoa)
                    .HasForeignKey(d => d.IdPessoaJuridica)
                    .HasConstraintName("FK_Pessoa_PessoaJuridica");
            });

            modelBuilder.Entity<PessoaFisica>(entity =>
            {
                entity.HasKey(e => e.IdPessoaFisica);

                entity.HasIndex(e => e.DsDocId)
                    .HasName("UQ__PessoaFi__C46211DB10A00ADD")
                    .IsUnique();

                entity.HasIndex(e => e.IdPessoaFisica)
                    .HasName("IXFK_PessoaFisica_PessoaFisica");

                entity.HasIndex(e => e.NuCpf)
                    .HasName("UQ_PessoaFisicaCPF")
                    .IsUnique();

                entity.Property(e => e.CdCvLattes).IsUnicode(false);

                entity.Property(e => e.CdEmail)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CdLinkedIn).IsUnicode(false);

                entity.Property(e => e.CdSexo)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DsComplemento)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsDocId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DsEndereco)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DtCriacao).HasColumnType("datetime");

                entity.Property(e => e.DtNascimento).HasColumnType("date");

                entity.Property(e => e.NmBairro)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NmCv)
                    .HasColumnName("NmCV")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NmPessoa)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NuCelular)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NuCep)
                    .HasColumnName("NuCEP")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.NuCpf)
                    .IsRequired()
                    .HasColumnName("NuCPF")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NuEndereco)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NuTelefoneComercial)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NuTelefoneFixo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SgUf)
                    .HasColumnName("SgUF")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCidadeNavigation)
                    .WithMany(p => p.PessoaFisica)
                    .HasForeignKey(d => d.IdCidade)
                    .HasConstraintName("FK_PessoaFisica_Cidade");

                entity.HasOne(d => d.IdTipoVinculoNavigation)
                    .WithMany(p => p.PessoaFisica)
                    .HasForeignKey(d => d.IdTipoVinculo)
                    .HasConstraintName("FK_PessoaFisica_TipoVinculo");

                entity.HasOne(d => d.IdUsuarioCriacaoNavigation)
                    .WithMany(p => p.PessoaFisica)
                    .HasForeignKey(d => d.IdUsuarioCriacao)
                    .HasConstraintName("FK__PessoaFis__IdUsu__20389C96");
            });

            modelBuilder.Entity<PessoaJuridica>(entity =>
            {
                entity.HasKey(e => e.IdPessoaJuridica);

                entity.HasIndex(e => e.Cnpj)
                    .HasName("UQ_PessoaJuridicaCNPJ")
                    .IsUnique()
                    .HasFilter("([CNPJ] IS NOT NULL)");

                entity.Property(e => e.Cep)
                    .HasColumnName("CEP")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Cnpj)
                    .HasColumnName("CNPJ")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Complemento)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DsInternacional)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DtCriacao).HasColumnType("datetime");

                entity.Property(e => e.Endereco)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NmBairro)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NmFantasia)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.NuEndereco)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RazaoSocial)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Uf)
                    .HasColumnName("UF")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCidadeNavigation)
                    .WithMany(p => p.PessoaJuridica)
                    .HasForeignKey(d => d.IdCidade)
                    .HasConstraintName("FK_PessoaJuridica_Cidade");

                entity.HasOne(d => d.IdClassificacaoEmpresaNavigation)
                    .WithMany(p => p.PessoaJuridica)
                    .HasForeignKey(d => d.IdClassificacaoEmpresa)
                    .HasConstraintName("FK_PessoaJuridica_ClassificacaoEmpresa");

                entity.HasOne(d => d.IdEntidadeNavigation)
                    .WithMany(p => p.PessoaJuridica)
                    .HasForeignKey(d => d.IdEntidade)
                    .HasConstraintName("FK_PessoaJuridica_Entidade");

                entity.HasOne(d => d.IdEsferaEmpresaNavigation)
                    .WithMany(p => p.PessoaJuridica)
                    .HasForeignKey(d => d.IdEsferaEmpresa)
                    .HasConstraintName("FK_PessoaJuridica_EsferaEmpresa");

                entity.HasOne(d => d.IdPaisNavigation)
                    .WithMany(p => p.PessoaJuridica)
                    .HasForeignKey(d => d.IdPais)
                    .HasConstraintName("FK_PessoaJuridica_Pais");

                entity.HasOne(d => d.IdTipoAdministracaoNavigation)
                    .WithMany(p => p.PessoaJuridica)
                    .HasForeignKey(d => d.IdTipoAdministracao)
                    .HasConstraintName("FK_PessoaJuridica_TipoAdministracao");

                entity.HasOne(d => d.IdUsuarioCriacaoNavigation)
                    .WithMany(p => p.PessoaJuridica)
                    .HasForeignKey(d => d.IdUsuarioCriacao)
                    .HasConstraintName("FK__PessoaJur__IdUsu__212CC0CF");
            });

            modelBuilder.Entity<Proposta>(entity =>
            {
                entity.HasKey(e => e.IdProposta);

                entity.HasIndex(e => e.IdTipoProposta)
                    .HasName("IXFK_Proposta_TipoProposta");

                entity.Property(e => e.IdProposta).ValueGeneratedNever();

                entity.Property(e => e.DsAditivo).IsUnicode(false);

                entity.Property(e => e.DsApelidoProposta)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.DsAssunto).IsUnicode(false);

                entity.Property(e => e.DsObjeto).IsUnicode(false);

                entity.Property(e => e.DsObservacao).IsUnicode(false);

                entity.Property(e => e.DtAssinaturaContrato).HasColumnType("date");

                entity.Property(e => e.DtAutorizacaoInicio).HasColumnType("date");

                entity.Property(e => e.DtCriacao).HasColumnType("datetime");

                entity.Property(e => e.DtLimiteEntregaProposta).HasColumnType("date");

                entity.Property(e => e.DtLimiteEnvioProposta).HasColumnType("date");

                entity.Property(e => e.DtNovoFimVigencia).HasColumnType("date");

                entity.Property(e => e.DtProposta).HasColumnType("date");

                entity.Property(e => e.DtUltimaAlteracao).HasColumnType("datetime");

                entity.Property(e => e.DtValidadeProposta).HasColumnType("date");

                entity.Property(e => e.NuContratoCliente)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NuPrazoEstimadoMes).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.NuPrazoEstimadoMesJuridico).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.VlAditivo).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlContrato).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlContratoComAditivo).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VlProposta).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.IdContratoNavigation)
                    .WithMany(p => p.Proposta)
                    .HasForeignKey(d => d.IdContrato)
                    .HasConstraintName("FK_Proposta_Contrato");

                entity.HasOne(d => d.IdFundamentoNavigation)
                    .WithMany(p => p.Proposta)
                    .HasForeignKey(d => d.IdFundamento)
                    .HasConstraintName("FK_Proposta_FundamentoContratacao");

                entity.HasOne(d => d.IdOportunidadeNavigation)
                    .WithMany(p => p.Proposta)
                    .HasForeignKey(d => d.IdOportunidade)
                    .HasConstraintName("FK_Proposta_Oportunidade");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.Proposta)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Proposta_Situacao");

                entity.HasOne(d => d.IdTemaNavigation)
                    .WithMany(p => p.Proposta)
                    .HasForeignKey(d => d.IdTema)
                    .HasConstraintName("FK_Proposta_Tema");

                entity.HasOne(d => d.IdTipoAditivoNavigation)
                    .WithMany(p => p.Proposta)
                    .HasForeignKey(d => d.IdTipoAditivo)
                    .HasConstraintName("FK_Proposta_TipoAditivo");

                entity.HasOne(d => d.IdTipoOportunidadeNavigation)
                    .WithMany(p => p.Proposta)
                    .HasForeignKey(d => d.IdTipoOportunidade)
                    .HasConstraintName("FK_Proposta_TipoOportunidade");

                entity.HasOne(d => d.IdTipoPropostaNavigation)
                    .WithMany(p => p.Proposta)
                    .HasForeignKey(d => d.IdTipoProposta)
                    .HasConstraintName("FK_Proposta_TipoProposta");

                entity.HasOne(d => d.IdTipoReajusteNavigation)
                    .WithMany(p => p.Proposta)
                    .HasForeignKey(d => d.IdTipoReajuste)
                    .HasConstraintName("FK_Proposta_IndiceReajuste");

                entity.HasOne(d => d.IdUnidadeTempoNavigation)
                    .WithMany(p => p.PropostaIdUnidadeTempoNavigation)
                    .HasForeignKey(d => d.IdUnidadeTempo)
                    .HasConstraintName("FK_Proposta_UnidadeDeTempo");

                entity.HasOne(d => d.IdUnidadeTempoJuridicoNavigation)
                    .WithMany(p => p.PropostaIdUnidadeTempoJuridicoNavigation)
                    .HasForeignKey(d => d.IdUnidadeTempoJuridico)
                    .HasConstraintName("FK_Proposta_UnidadeDeTempoJuridico");

                entity.HasOne(d => d.IdUsuarioCriacaoNavigation)
                    .WithMany(p => p.Proposta)
                    .HasForeignKey(d => d.IdUsuarioCriacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Proposta_Usuario");
            });

            modelBuilder.Entity<PropostaCliente>(entity =>
            {
                entity.HasKey(e => e.IdPropostaCliente);

                entity.Property(e => e.NmFantasia)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RazaoSocial)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.PropostaCliente)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PropostaCliente_Cliente");

                entity.HasOne(d => d.IdPropostaNavigation)
                    .WithMany(p => p.PropostaCliente)
                    .HasForeignKey(d => d.IdProposta)
                    .HasConstraintName("FK_PropostaCliente_Proposta");
            });

            modelBuilder.Entity<PropostaComentario>(entity =>
            {
                entity.HasKey(e => e.IdPropostaComentario);

                entity.Property(e => e.DsComentario)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.DtComentario).HasColumnType("datetime");

                entity.HasOne(d => d.IdPropostaNavigation)
                    .WithMany(p => p.PropostaComentario)
                    .HasForeignKey(d => d.IdProposta)
                    .HasConstraintName("FK_PropostaComentario_Proposta");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.PropostaComentario)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PropostaComentario_Usuario");
            });

            modelBuilder.Entity<PropostaComentarioLido>(entity =>
            {
                entity.HasKey(e => e.IdPropostaComentarioLido);
            });

            modelBuilder.Entity<PropostaContato>(entity =>
            {
                entity.HasKey(e => e.IdPropostaContato);

                entity.Property(e => e.CdEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NmContato)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NmDepartamento)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.NuCelular)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NuTelefone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdTipoContatoNavigation)
                    .WithMany(p => p.PropostaContato)
                    .HasForeignKey(d => d.IdTipoContato)
                    .HasConstraintName("FK_PropostaContato_TipoContato");
            });

            modelBuilder.Entity<PropostaCoordenador>(entity =>
            {
                entity.HasKey(e => e.IdPropostaCoordenador);

                entity.HasOne(d => d.IdPessoaNavigation)
                    .WithMany(p => p.PropostaCoordenador)
                    .HasForeignKey(d => d.IdPessoa)
                    .HasConstraintName("FK_PropostaCoordenador_Pessoa");

                entity.HasOne(d => d.IdPropostaNavigation)
                    .WithMany(p => p.PropostaCoordenador)
                    .HasForeignKey(d => d.IdProposta)
                    .HasConstraintName("FK_PropostaCoordenador_Proposta");
            });

            modelBuilder.Entity<PropostaDocs>(entity =>
            {
                entity.HasKey(e => e.IdPropostaDocs);

                entity.HasIndex(e => e.DocFisicoId)
                    .HasName("UQ__Proposta__66E037B219F865B0")
                    .IsUnique();

                entity.Property(e => e.DocFisico).IsRequired();

                entity.Property(e => e.DocFisicoId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DsDoc)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DtUpLoad).HasColumnType("date");

                entity.Property(e => e.NmCriador)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NmDocumento)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdPropostaNavigation)
                    .WithMany(p => p.PropostaDocs)
                    .HasForeignKey(d => d.IdProposta)
                    .HasConstraintName("FK_PropostaDocs_Proposta");

                entity.HasOne(d => d.IdTipoDocNavigation)
                    .WithMany(p => p.PropostaDocs)
                    .HasForeignKey(d => d.IdTipoDoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PropostaDocs_TipoDocumento");
            });

            modelBuilder.Entity<PropostaDocsPrincipais>(entity =>
            {
                entity.HasKey(e => e.IdPropostaDocsPrincipais);

                entity.HasIndex(e => e.DocFisicoId)
                    .HasName("UQ__Proposta__66E037B21B49E96B")
                    .IsUnique();

                entity.Property(e => e.DocFisico).IsRequired();

                entity.Property(e => e.DocFisicoId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DsDoc)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DtUpLoad).HasColumnType("datetime");

                entity.Property(e => e.NmCriador)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NmDocumento)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PropostaHistorico>(entity =>
            {
                entity.HasKey(e => e.IdPropostaHistorico);

                entity.Property(e => e.DtFim).HasColumnType("datetime");

                entity.Property(e => e.DtInicio).HasColumnType("datetime");

                entity.HasOne(d => d.IdPropostaNavigation)
                    .WithMany(p => p.PropostaHistorico)
                    .HasForeignKey(d => d.IdProposta)
                    .HasConstraintName("FK_PropostaHistorico_Proposta");

                entity.HasOne(d => d.IdSituacaoNavigation)
                    .WithMany(p => p.PropostaHistorico)
                    .HasForeignKey(d => d.IdSituacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PropostaHistorico_Situacao");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.PropostaHistorico)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PropostaHistorico_Usuario");
            });

            modelBuilder.Entity<Situacao>(entity =>
            {
                entity.HasKey(e => e.IdSituacao);

                entity.Property(e => e.DsArea)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsSituacao)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsSubArea)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IcEntidade)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.IcNfemitida).HasColumnName("IcNFEmitida");
            });

            modelBuilder.Entity<TaxaInstitucional>(entity =>
            {
                entity.HasKey(e => e.IdTaxaInstitucional);

                entity.Property(e => e.DsTaxaInstitucional)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PcTaxaInstitucional).HasColumnType("decimal(4, 2)");
            });

            modelBuilder.Entity<Tema>(entity =>
            {
                entity.HasKey(e => e.IdTema);

                entity.Property(e => e.DsTema)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoAditivo>(entity =>
            {
                entity.HasKey(e => e.IdTipoAditivo);

                entity.Property(e => e.IdTipoAditivo).ValueGeneratedNever();

                entity.Property(e => e.DsTipoAditivo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoAdministracao>(entity =>
            {
                entity.HasKey(e => e.IdTipoAdministracao);

                entity.Property(e => e.DsTipoAdministracao)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoApresentacaoRelatorio>(entity =>
            {
                entity.HasKey(e => e.IdTipoApresentacaoRelatorio);

                entity.Property(e => e.DsTipoApresentacaoRelatorio)
                    .IsRequired()
                    .HasColumnName("dsTipoApresentacaoRelatorio")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoCobranca>(entity =>
            {
                entity.HasKey(e => e.IdTipoCobranca);

                entity.Property(e => e.DsTipoCobranca)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoContato>(entity =>
            {
                entity.HasKey(e => e.IdTipoContato);

                entity.Property(e => e.DsTipoContato)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoContratacaoEquipeTecnica>(entity =>
            {
                entity.HasKey(e => e.IdTipoContratacao)
                    .HasName("PK_TipoContratacao");

                entity.Property(e => e.DsTipoContratacao)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoCoordenacao>(entity =>
            {
                entity.HasKey(e => e.IdTipoCoordenacao);

                entity.Property(e => e.DsTipoCoordenacao)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoDocsAcompanhaNf>(entity =>
            {
                entity.HasKey(e => e.IdTipoDocsAcompanhaNf);

                entity.ToTable("TipoDocsAcompanhaNF");

                entity.Property(e => e.IdTipoDocsAcompanhaNf).HasColumnName("IdTipoDocsAcompanhaNF");

                entity.Property(e => e.DsTipoDocsAcompanhaNf)
                    .IsRequired()
                    .HasColumnName("DsTipoDocsAcompanhaNF")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IcPadrao).HasColumnName("icPadrao");
            });

            modelBuilder.Entity<TipoDocumento>(entity =>
            {
                entity.HasKey(e => e.IdTipoDoc);

                entity.Property(e => e.DsTipoDoc)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoEntidade>(entity =>
            {
                entity.HasKey(e => e.IdTipoEntidade)
                    .HasName("PK__TipoEnti__59D7BBC8F69732AA");

                entity.Property(e => e.DsTipoEntidade)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoEntregaDocumento>(entity =>
            {
                entity.HasKey(e => e.IdTipoEntregaDocumento);

                entity.Property(e => e.IdTipoEntregaDocumento).HasColumnName("idTipoEntregaDocumento");

                entity.Property(e => e.DsTipoEntregaDocumento)
                    .IsRequired()
                    .HasColumnName("dsTipoEntregaDocumento")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoOportunidade>(entity =>
            {
                entity.HasKey(e => e.IdTipoOportunidade)
                    .HasName("PK_GrauOportunidade");

                entity.Property(e => e.DsTipoOportunidade)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IcModulo)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoPessoa>(entity =>
            {
                entity.HasKey(e => e.IdTipoPessoa);

                entity.Property(e => e.DsTipoPessoa)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoProposta>(entity =>
            {
                entity.HasKey(e => e.IdTipoProposta);

                entity.Property(e => e.DsTipoProposta)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoVinculo>(entity =>
            {
                entity.HasKey(e => e.IdTipoVinculo)
                    .HasName("PK__TipoVinc__7D8836D8EE9B67ED");

                entity.Property(e => e.DsTipoVinculo)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UnidadeDeTempo>(entity =>
            {
                entity.HasKey(e => e.IdUnidadeTempo)
                    .HasName("PK_UnidadeTempo");

                entity.Property(e => e.DsUnidadeTempo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);

                entity.Property(e => e.CdSenha)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DsLogin)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdPessoaNavigation)
                    .WithMany(p => p.Usuario)
                    .HasForeignKey(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_PessoaFisica");
            });

            modelBuilder.Entity<VinculoPessoaFisica>(entity =>
            {
                entity.HasKey(e => e.IdVinculoPessoa)
                    .HasName("PK__VinculoP__063C0E6D094B019D");

                entity.Property(e => e.DtFimVinculo).HasColumnType("date");

                entity.Property(e => e.DtInicioVinculo).HasColumnType("date");

                entity.HasOne(d => d.IdPessoaFisicaNavigation)
                    .WithMany(p => p.VinculoPessoaFisica)
                    .HasForeignKey(d => d.IdPessoaFisica)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VinculoPessoaFisica_PessoaFisica");

                entity.HasOne(d => d.IdTipoVinculoNavigation)
                    .WithMany(p => p.VinculoPessoaFisica)
                    .HasForeignKey(d => d.IdTipoVinculo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VinculoPessoaFisica_TipoVinculo");
            });
        }
    }
}
