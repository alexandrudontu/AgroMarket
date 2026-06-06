import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef } from '@angular/core';
import { DatePipe } from '@angular/common';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import {
  trigger,
  state,
  style,
  transition,
  animate
} from '@angular/animations';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css'],
  imports: [DatePipe, CommonModule, RouterModule],
  animations: [

  trigger('expandAnimation', [

    state('collapsed', style({
      height: '0px',
      opacity: 0,
      overflow: 'hidden'
    })),

    state('expanded', style({
      height: '*',
      opacity: 1
    })),

    transition('collapsed <=> expanded', [
      animate('250ms ease')
    ])
  ])
]
})
export class OrdersComponent implements OnInit {

  orders: any[] = [];
  expandedOrderId: number | null = null;
  apiUrl = environment.apiUrl;

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.http.get(this.apiUrl + '/api/orders/customer')
      .subscribe((res: any) => {
        this.orders = res;
        this.cdr.detectChanges();
      });
  }

  toggleOrder(id: number) {
    if (this.expandedOrderId === id) {

      this.expandedOrderId = null;
      return;
    }

    this.expandedOrderId = id;
  }
}
