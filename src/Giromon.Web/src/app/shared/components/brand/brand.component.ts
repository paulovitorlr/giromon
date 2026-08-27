import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-brand', imports: [RouterLink],
  template: `<a class="brand" routerLink="/" aria-label="Página inicial do Giromon"><span class="orb" aria-hidden="true"><i></i></span><b>GIRO<span>MON</span></b></a>`,
  styles: [`.brand{display:flex;align-items:center;gap:.7rem;color:#fff;text-decoration:none}.brand b{font-weight:950;letter-spacing:-.06em;font-size:1.55rem;font-style:italic}.brand b span{color:#ffca3a}.orb{width:2rem;aspect-ratio:1;border-radius:50%;display:grid;place-items:center;background:linear-gradient(#ff5364 0 43%,#101528 43% 57%,#f7f8ff 57%);box-shadow:0 0 22px #ffca3a55}.orb i{width:.52rem;aspect-ratio:1;border-radius:50%;background:#ffca3a;border:3px solid #101528}`]
})
export class BrandComponent {}
