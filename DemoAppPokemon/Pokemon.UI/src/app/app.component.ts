import { Component } from '@angular/core';
import { Pokemon } from './models/Pokemon';
import { PokemonService } from './services/pokemon.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Pokemon.UI';

  pokemon : Pokemon[]=[];

constructor(private PokemonService: PokemonService){}

  ngOnInit():void{
    this.PokemonService.getPokemon().subscribe((result: Pokemon[])=> (this.pokemon = result));
  }
}
