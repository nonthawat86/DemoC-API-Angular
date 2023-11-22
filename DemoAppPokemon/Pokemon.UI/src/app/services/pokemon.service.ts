import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Pokemon } from '../models/Pokemon';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class PokemonService {

  private url = "Pokemon";

  constructor(private http : HttpClient) { }
  public getPokemon(): Observable<Pokemon[]>{
   
    return this.http.get<Pokemon[]>(`${environment.apiUrl}/${this.url}`);
  }
}
