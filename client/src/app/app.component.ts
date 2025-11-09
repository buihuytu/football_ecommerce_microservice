import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    NavbarComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{
  title = 'eShopping';
  products: any[] = [];

  constructor(
    private httpClient: HttpClient
  ) {
    
  }

  ngOnInit(): void {
    this.httpClient.get('http://localhost:8000/api/v1/Catalog/GetProductsByBrandName/Adidas')
      .subscribe((res: any) => {
        this.products = res,
        console.log(res)
      })
  }
}
