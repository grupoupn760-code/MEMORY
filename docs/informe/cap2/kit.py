import html
from playwright.sync_api import sync_playwright
TERM="#B3A2DC"; PROC="#CDD3DD"; ROMBO="#4A2C7E"; FL="#7A7782"; TXT="#1F1B24"; SUB="#4A4652"
class D:
    def __init__(s,w,h): s.w=w; s.h=h; s.e=[]
    def t(s,x,y,txt,size=12,bold=False,color=TXT,anchor="middle"):
        s.e.append(f'<text x="{x}" y="{y}" font-size="{size}" fill="{color}" text-anchor="{anchor}" font-weight="{"700" if bold else "400"}">{html.escape(txt)}</text>')
    def box(s,cx,y,w,h,tit,sub=None,term=False):
        rx=12 if term else 3
        s.e.append(f'<rect x="{cx-w/2}" y="{y}" width="{w}" height="{h}" rx="{rx}" fill="{TERM if term else PROC}"/>')
        if sub: s.t(cx,y+h/2-2,tit,13,True); s.t(cx,y+h/2+14,sub,11,color=SUB)
        else: s.t(cx,y+h/2+5,tit,13,True)
    def rombo(s,cx,cy,w,h,l):
        s.e.append(f'<polygon points="{cx},{cy-h/2} {cx+w/2},{cy} {cx},{cy+h/2} {cx-w/2},{cy}" fill="{ROMBO}"/>')
        y0=cy-(len(l)-1)*7+4
        for i,x in enumerate(l): s.t(cx,y0+14*i,x,11.5,color="#FFFFFF")
    def fl(s,pts,label=None,lx=None,ly=None):
        d="M"+" L".join(f"{x},{y}" for x,y in pts)
        s.e.append(f'<path d="{d}" fill="none" stroke="{FL}" stroke-width="2" marker-end="url(#m)"/>')
        if label: s.t(lx,ly,label,11.5,anchor="start")
    def png(s,ruta,esc=2.5):
        svg=(f'<svg xmlns="http://www.w3.org/2000/svg" width="{s.w}" height="{s.h}" font-family="Liberation Sans, Arial, sans-serif">'
             f'<defs><marker id="m" markerWidth="8" markerHeight="8" refX="7" refY="4" orient="auto"><path d="M0,0 L8,4 L0,8 z" fill="{FL}"/></marker></defs>'
             f'<rect width="{s.w}" height="{s.h}" fill="white"/>'+''.join(s.e)+'</svg>')
        with sync_playwright() as p:
            b=p.chromium.launch(); pg=b.new_page(viewport={"width":s.w,"height":s.h},device_scale_factor=esc)
            pg.set_content('<html><body style="margin:0">'+svg+'</body></html>'); pg.locator("svg").screenshot(path=ruta); b.close()
