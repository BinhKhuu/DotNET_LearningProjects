import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  constructor(
    private httpClient: HttpClient
  ){

  }

  callApi(){
    this.httpClient.get("http://localhost:5198/weatherforecast")
    .subscribe(result => console.log('results are: ', result))
  }
}
