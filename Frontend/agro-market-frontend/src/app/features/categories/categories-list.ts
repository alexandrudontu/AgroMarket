import { Component, OnInit } from '@angular/core';
import { CategoriesService } from './categories.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'categories-list',
  templateUrl: './categories-list.html',
  styleUrls: ['./categories-list.css'],
  imports: [RouterModule, CommonModule] 
})
export class CategoriesListComponent implements OnInit {

  categories: any = [];

  constructor(private service: CategoriesService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.service.getAll().subscribe(res => {
      this.categories = res;
      this.cdr.detectChanges();
    });
  }

  getCategoryEmoji(category: string): string {
    const name = category.toLowerCase();

    if (name.includes('fructe'))
      return '🍎';

    if (name.includes('legume'))
      return '🥦';

    if (name.includes('lactate'))
      return '🥛';

    if (name.includes('carne'))
      return '🥩';

    if (name.includes('pâine'))
      return '🍞';

    if (name.includes('băuturi'))
      return '🍹';

    if (name.includes('brânzeturi'))
      return '🧀';

    if (name.includes('cereale'))
      return '🌾';

    if (name.includes('plante'))
      return '🪴';

    return '🛒';
  }
}