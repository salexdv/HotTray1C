using System.Xml;
using System.Windows.Forms;
using System.Drawing;

namespace MainStruct
{
    public struct СтруктураНастроекЭлемента
    {
        public XmlNode Группа;
        public XmlNode Ссылка;
        public bool Приложение;
        public string Наименование;
        public string Путь;
        public string ИмяПользователя;
        public string Пароль;
        public string Описание;
        public string СочетаниеКлавиш;
        public string ПутьКХранилищу;
        public string ИмяПользователяХранилища;
        public string ПарольПользователяХранилища;
        public string ПрограммаЗапуска;
        public string КодДоступа;
        public string ВерсияПлатформы;
        public bool РежимЗапускаКакПунктМеню;
        public bool ВидКлиентаКакПунктМеню;
        public bool ПоказыватьВМенюЗапуска;
        public bool ИспользуетсяАутентификацияWindows;
        public int ТипБазы;
        public int ТипПлатформы;
        public int РежимЗапуска;
        public int РежимРаботы;
        public int ВидКлиента;
        public ListView ДополнительныеПользователи;
    }

    public struct СтруктураНастроекГруппыЭлементов
    {
        public XmlNode Группа;
        public XmlNode Ссылка;
        public string Наименование;
        public string Описание;
        public bool ГруппаРаскрыта;
        public bool ГруппаАктивна;
        public string СочетаниеКлавиш;
        public int Действие;
    }

    public struct СтруктураНастроекОбновления
    {        
        public string Сервер;
        public int Порт;
        public int Тип;
        public string Пользователь;
        public string Пароль;
    }

    public struct СтруктураСобственныхНастроекМеню
    {
        public Color ЦветФона;
        public Color ЦветШрифта;
        public Color ЦветСепаратора;
        public int РазмерШрифта;
        public FontStyle НачертаниеШрифта;
    }
    
    public struct StructChangeStateIbases
    {
    	public int BaseType;
    	public string BasePath;
    	public bool DelFromFile;
    	public bool ChangeVersion;
    	public string OriginalVersion;
    	public bool ChangeAppID;
    	public string OriginalAppID;
    }
}