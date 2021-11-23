namespace WCFLoginUniversal
{
    public class PregFrec
    {
        int _idpf;
        int _idusuario;
        bool _hacepf;

        public int IdPF
        {
            get
            {
                return _idpf;
            }

            set
            {
                _idpf = value;
            }
        }

        public int IdUsuario
        {
            get
            {
                return _idusuario;
            }

            set
            {
                _idusuario = value;
            }
        }

        public bool HacePF
        {
            get
            {
                return _hacepf;
            }

            set
            {
                _hacepf = value;
            }
        }
    }
}
