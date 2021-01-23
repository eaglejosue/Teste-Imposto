using System;
using System.Data;
using System.Windows.Forms;
using TesteImposto.Domain.Model;
using TesteImposto.Domain.Service;
using TesteImposto.Domain.Util;

namespace TesteImposto
{
    public partial class FormImposto : Form
    {
        public FormImposto()
        {
            InitializeComponent();
            InicializarGridView();
            InicializarCampos();
        }

        private void InicializarGridView()
        {
            dataGridViewPedidos.AutoGenerateColumns = true;
            dataGridViewPedidos.DataSource = GetTablePedidos();

            ((DataGridViewTextBoxColumn)dataGridViewPedidos.Columns["Nome do produto"]).MaxInputLength = 50;
            ((DataGridViewTextBoxColumn)dataGridViewPedidos.Columns["Codigo do produto"]).MaxInputLength = 20;

            ResizeColumns();
        }

        private void ButtonGerarNotaFiscal_Click(object sender, EventArgs e)
        {
            var pedido = new Pedido(textBoxNomeCliente.Text, comboBoxEstadoOrigem.Text, comboBoxEstadoDestino.Text);
            var table = (DataTable)dataGridViewPedidos.DataSource;

            // Obtém itens da grid
            foreach (DataRow row in table.Rows)
            {
                var nome = row["Nome do produto"] == null ? string.Empty : row["Nome do produto"].ToString();
                var codigo = row["Codigo do produto"] == null ? string.Empty : row["Codigo do produto"].ToString();
                var valor = row["Valor"] == null ? 0M : Convert.ToDecimal(row["Valor"].ToString());
                var brinde = row["Brinde"] != null && row["Brinde"].ToString() == "True";

                pedido.ItensDoPedido.Add(new PedidoItem(nome, codigo, valor, brinde));
            }

            try
            {
                if (!pedido.ValidarPedido(out var mensagemValidacao))
                {
                    MessageBox.Show(mensagemValidacao, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var service = new NotaFiscalService();
                if (service.GerarNotaFiscal(pedido))
                {
                    MessageBox.Show("Operação efetuada com sucesso!");
                    LimparCampos();
                }
                else
                {
                    MessageBox.Show("Ocorreram erros ao gerar a nota fiscal!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível completar a operação - " + ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InicializarCampos()
        {
            comboBoxEstadoOrigem.DataSource = EstadosDisponiveis.ListaDeEstadosDeOrigem;
            comboBoxEstadoDestino.DataSource = EstadosDisponiveis.ListaDeEstadosDeDestino;
            comboBoxEstadoOrigem.Text = "Selecione";
            comboBoxEstadoDestino.Text = "Selecione";
        }

        private object GetTablePedidos()
        {
            var table = new DataTable("pedidos");
            table.Columns.Add(new DataColumn("Nome do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Codigo do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Valor", typeof(decimal)));
            table.Columns.Add(new DataColumn("Brinde", typeof(bool)));
            return table;
        }

        private void ResizeColumns()
        {
            var mediaWidth = dataGridViewPedidos.Width / dataGridViewPedidos.Columns.GetColumnCount(DataGridViewElementStates.Visible);

            for (int i = dataGridViewPedidos.Columns.Count - 1; i >= 0; i--)
            {
                var coluna = dataGridViewPedidos.Columns[i];
                coluna.Width = Convert.ToInt32(mediaWidth);
            }
        }

        private void LimparCampos()
        {
            // Exercício 6
            textBoxNomeCliente.Text = "";
            comboBoxEstadoOrigem.Text = "Selecione";
            comboBoxEstadoDestino.Text = "Selecione";

            InicializarGridView();

            textBoxNomeCliente.Focus();
        }

        private void DataGridViewPedidos_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                var controle = e.Control as DataGridViewTextBoxEditingControl;
                if (controle != null)
                {
                    controle.Enter -= TirarMascara;
                    controle.Leave -= RetornarMascara;
                    controle.KeyPress -= ApenasValorNumerico;
                    if (((DataGridView)sender).CurrentCell.ColumnIndex == 0)
                    {
                        controle.MaxLength = 50;
                    }
                    if (((DataGridView)sender).CurrentCell.ColumnIndex == 1)
                    {
                        controle.MaxLength = 20;
                    }
                    if (((DataGridView)sender).CurrentCell.ColumnIndex == 2)
                    {
                        controle.MaxLength = 18;
                        controle.Enter += TirarMascara;
                        controle.Leave += RetornarMascara;
                        controle.KeyPress += ApenasValorNumerico;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void RetornarMascara(object sender, EventArgs e)
        {
            try
            {
                var txt = (TextBox)sender;
                txt.Text = double.Parse(txt.Text.Length == 1 && txt.Text.Equals(",") ? "0" : txt.Text).ToString("C2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void TirarMascara(object sender, EventArgs e)
        {
            try
            {
                var txt = (TextBox)sender;
                txt.Text = txt.Text.Replace("R$", "").Trim();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void ApenasValorNumerico(object sender, KeyPressEventArgs e)
        {
            try
            {
                var txt = (TextBox)sender;
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Back))
                {
                    if (e.KeyChar == ',')
                    {
                        e.Handled = (txt.Text.Contains(","));
                    }
                    else
                        e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
