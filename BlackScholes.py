import numpy as np
import pandas as pd
import scipy.stats as st
import math
import yfinance as yf
from bs4 import BeautifulSoup
import requests

def black_scholes(stock, X, exp):
    #Interest Rate
    url = 'https://www.cnbc.com/quotes/US10Y'
    res = requests.get(url)
    soup = BeautifulSoup(res.content, 'lxml')
    rfr = soup.findAll('span', class_ = 'QuoteStrip-lastPrice')
    for x in rfr:
        x = x.text.replace('%','')
        rfr = x
        rfr = float(rfr)
    print(f"Current Interest Rate: {rfr}")

    #STOCK PRICE
    def getprice(stock):
        stock = yf.Ticker(stock)
        price = stock.info['currentPrice']
        return price

    #VOLATILITY
    def getvolatility(stock):
        stock = stock.upper()
        url = f'https://www.alphaquery.com/stock/{stock}/volatility-option-statistics/180-day/historical-volatility'
        res = requests.get(url)
        soup = BeautifulSoup(res.content, 'lxml')
        volatility = soup.findAll('div', class_ = "indicator-figure-inner")[0]
        vol = volatility.text
        return vol

    vol = getvolatility(stock)
    price = getprice(stock)
    vol = float(vol)
    
    print(f"Current stock price: {price}")
    print(f"Volatility: {vol*100}%")
    
    #GREEKS 
    t = exp/365
    v = vol
    r = rfr/100
    S = price

    d1 = (np.log(S/X)+(r + .5*v**2)*t) / (v*math.sqrt(t))
    d2 = d1 - v*math.sqrt(t)

    call_price = round(S*(st.norm.cdf(d1)) - (X/(math.exp(r*t)))*(st.norm.cdf(d2)), 2)
    put_price = round(call_price + X/(math.exp(r*t)) - S, 2)

    call_delta = round(math.exp((r-r)*t) * st.norm.cdf(d1), 2)
    put_delta = round(math.exp((r-r)*t) * (st.norm.cdf(d1)-1), 2)
    
    call_gamma = round((math.exp((r-r)*t) * st.norm.pdf(d1)) / (S * v*math.sqrt(t)), 2)
    put_gamma = round((math.exp((r-r)*t) * st.norm.pdf(d1)) / (S * v*math.sqrt(t)), 2)
    
    call_theta = round((-S *st.norm.pdf(d1)*v/(2*math.sqrt(t)) - r*X*math.exp(-r*t)*st.norm.cdf(d2))/365, 2)
    put_theta = round((-S *st.norm.pdf(d1)*v/(2*math.sqrt(t)) + r*X*math.exp(-r*t)*st.norm.cdf(-d2))/365, 2)
    
    call_vega = round((S*math.exp((r-r)*t)*st.norm.pdf(d1)*math.sqrt(t))/100, 2)
    put_vega = round((S*math.exp((r-r)*t)*st.norm.pdf(d1)*math.sqrt(t))/100, 2)
    
    callbreakeven = call_price + X
    putbreakeven = X - put_price

    #TABLE
    callsnputs = {
        'Stock Price': [S],
        'Strike Price': [X],
        'Breakeven': [callbreakeven, putbreakeven],
        'Option Price': [call_price, put_price], 
        'Delta': [call_delta, put_delta], 
        'Gamma': [call_gamma, put_gamma],
        'Vega': [call_vega, put_vega], 
        'Theta': [call_theta, put_theta],
        }
    rows = "Calls", "Puts"
    data = pd.DataFrame(callsnputs, rows)
    return data

a = input("Stock ticker: ")
b = float(input("Strike price: "))
c = float(input("Time to expiration: (in days) ")) 

print(black_scholes(a, b, c))
