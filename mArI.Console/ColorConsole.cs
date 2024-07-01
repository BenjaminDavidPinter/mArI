public static class ColorConsole {
    public static void WriteLine(string text, ConsoleColor? fgColor = null, ConsoleColor? bgColor = null){
        
        var previousFgColor = Console.ForegroundColor;
        if(fgColor != null){
            Console.ForegroundColor = fgColor.Value;
        }
        
        var previousBgColor = Console.BackgroundColor;
        if(bgColor != null){
            Console.BackgroundColor = bgColor.Value;
        }

        Console.WriteLine(text);

        if(fgColor != null){
            Console.ForegroundColor = previousFgColor;
        }
        
        if(bgColor != null){
            Console.BackgroundColor = previousBgColor;
        }
    }

    public static void Write(string text, ConsoleColor? fgColor = null, ConsoleColor? bgColor = null){
        var previousFgColor = Console.ForegroundColor;
        if(fgColor != null){
            Console.ForegroundColor = fgColor.Value;
        }
        
        var previousBgColor = Console.BackgroundColor;
        if(bgColor != null){
            Console.BackgroundColor = bgColor.Value;
        }

        Console.Write(text);

        if(fgColor != null){
            Console.ForegroundColor = previousFgColor;
        }
        
        if(bgColor != null){
            Console.BackgroundColor = previousBgColor;
        }
    }
}