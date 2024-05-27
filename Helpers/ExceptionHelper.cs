namespace CarManagement.Helpers
{
    public static class ExceptionHelper
    {
        public static string GetFriendlyErrorMessage(Exception ex)
        {
            if (ex.Message.Contains("REFERENCE constraint"))
            {
                return "Não foi possível realizar a operação porque há dependências associadas. Por favor, remova as associações antes de tentar novamente.";
            }
            // Adicione mais casos aqui conforme necessário
            return ex.Message;
        }
    }

}
