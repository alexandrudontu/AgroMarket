import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef } from '@angular/core';
import { DatePipe } from '@angular/common';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css'],
  imports: [DatePipe, CommonModule, RouterModule]
})
export class OrdersComponent implements OnInit {

  orders: any[] = [];

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.http.get('https://localhost:7183/api/orders/customer')
      .subscribe((res: any) => {
        this.orders = res;
        this.cdr.detectChanges();
      });
  }
}
