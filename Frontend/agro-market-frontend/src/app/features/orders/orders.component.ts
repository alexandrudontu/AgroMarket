import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html'
})
export class OrdersComponent implements OnInit {

  orders: any[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get('https://localhost:7183/api/orders/customer')
      .subscribe((res: any) => {
        this.orders = res;
      });
  }
}
