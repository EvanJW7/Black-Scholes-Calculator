using System;

public class BlackScholes
{
	public static double calculateD1(double P, double X, double rfr, double vol, double t){
        return (Math.Log(P/X) + (rfr + .5 * Math.Pow(vol, 2))*(t)) / (vol * Math.Sqrt(t));
    }

    public static double calculateD2(double d1, double vol, double t){
        return d1 - vol * Math.Sqrt(t); 
    }

    public static double st_norm(double x){
        int neg = (x < 0d) ? 1 : 0;
        if ( neg == 1) 
            x *= -1d;

        double k = (1d / ( 1d + 0.2316419 * x));
        double y = (((( 1.330274429 * k - 1.821255978) * k + 1.781477937) * k - 0.356563782) * k + 0.319381530) * k;
        y = 1.0 - 0.398942280401 * Math.Exp(-0.5 * x * x) * y;

        return (1d - neg) * y + neg * (1d - y);
    }

    public static double calculateCP(double P, double d1, double X, double rfr, double d2, double t){
        return P * (st_norm(d1)) - X/(Math.Exp(rfr * t)) * (st_norm(d2));
    }

    public static double calculatePP(double call_price, double P, double X, double rfr, double t){
        return call_price + X/Math.Exp(rfr*t) - P;
    }

    public static double calculateCD(double d1, double rfr, double t){
        return (Math.Exp(rfr-rfr)*t) * st_norm(d1);
    }

    public static double calculatePD(double d1, double rfr, double t){
        return (Math.Exp(rfr-rfr)*t) * (st_norm(d1)-1);
    }

    public static double calculateCG(double d1, double P, double rfr, double vol, double t){
        return (Math.Exp(rfr-rfr)*t) * st_norm(d1) / (P * vol*Math.Sqrt(t));
    }

    public static double calculatePG(double d1, double P, double rfr, double vol, double t){
        return (Math.Exp(rfr-rfr)*t) * st_norm(d1) / (P * vol*Math.Sqrt(t));
    }

    public static double calculateCT(double d1, double d2, double P, double X, double rfr, double vol, double t){
        return (-P * st_norm(d1)*vol/(2*Math.Sqrt(t)) - rfr*X*Math.Exp(-rfr*t)*st_norm(d2))/365;
    }

    public static double calculatePT(double d1, double d2, double P, double X, double rfr, double vol, double t){
        return (-P * st_norm(d1)*vol/(2*Math.Sqrt(t)) + rfr*X*Math.Exp(-rfr*t)*st_norm(d2))/365;
    }

    public static double calculateCV(double P, double d1, double rfr, double t){
        return (P*Math.Exp((rfr-rfr)*t) * st_norm(d1) * Math.Sqrt(t)) / 100;
    }

    public static double calculatePV(double P, double d1, double rfr, double t){
        return (P*Math.Exp((rfr-rfr)*t) * st_norm(d1) * Math.Sqrt(t)) / 100;
    }
	
	public static void Main(string[] args)
	{
		double P, X, t, vol, rfr;

        //Settings
        //P = Stock Price, X = Strike Price, t = days to expiration, vol = volatility, rfr = real free rate
        P = 62;
        X = 60;
        t = 40;
        t = t/365;
        vol = .32;
        rfr = .04;
		
		double d1 = calculateD1(P, X, rfr, vol, t);
        double d2 = calculateD2(d1, vol, t);
        double call_price = calculateCP(P, d1, X, rfr, d2, t);
        double put_price = calculatePP(call_price, P, X, rfr, t);
        double call_delta = calculateCD(d1, rfr, t);
        double put_delta = calculatePD(d1, rfr, t);
        double call_gamma = calculateCG(d1, P, rfr, vol, t);
        double put_gamma = calculatePG(d1, P, rfr, vol, t);
        double call_theta = calculateCT(d1, d2, P, X, rfr, vol, t);
        double put_theta = calculatePT(d1, d2, P, X, rfr, vol, t);
        double call_vega = calculateCV(P, d1, rfr, t);
        double put_vega = calculatePV(P, d1, rfr, t);
        double call_breakeven = call_price + X;
        double put_breakeven = X - put_price;
		
	Console.WriteLine("Stock price: " + P);
        Console.WriteLine("Strike price: " + X);
        Console.WriteLine("Interest rate: " + (rfr*100) + '%');
        Console.WriteLine("Days to expiration: " + t*365);
        Console.WriteLine("Volatility: " + (vol*100) + '%');
	Console.WriteLine();
        Console.WriteLine("Call Breakeven: " + Math.Round(call_breakeven*100.0)/ 100.0);
        Console.WriteLine("Put Breakeven: " + Math.Round(put_breakeven*100.0)/ 100.0);
        Console.WriteLine("Call Price: " + Math.Round(call_price*100.0)/ 100.0);
        Console.WriteLine("Put Price: " + Math.Round(put_price*100.0)/ 100.0);
        Console.WriteLine("Call Delta: " + Math.Round(call_delta*100.0)/ 100.0);
        Console.WriteLine("Put Delta: " + Math.Round(put_delta*100.0)/ 100.0);
        Console.WriteLine("Call Gamma: " + Math.Round(call_gamma*100.0)/ 100.0);
        Console.WriteLine("Put Gamma: " + Math.Round(put_gamma*100.0)/ 100.0);
        Console.WriteLine("Call Vega: " + Math.Round(call_vega*100.0)/ 100.0);
        Console.WriteLine("Put Vega: " + Math.Round(put_vega*100.0)/ 100.0);
        Console.WriteLine("Call theta: " + Math.Round(call_theta*100.0)/ 100.0);
        Console.WriteLine("Put theta: " + Math.Round(put_theta*100.0)/ 100.0);
	}
}
