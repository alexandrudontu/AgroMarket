import { Component, OnInit } from '@angular/core';
import { ProductsService } from '../../products/products.service';
import { CategoriesService } from '../../categories/categories.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import {
  trigger,
  state,
  style,
  transition,
  animate
} from '@angular/animations';

@Component({
  selector: 'app-farmer-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
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
        animate('300ms ease')
      ])
    ])
  ]
})
export class FarmerDashboardComponent implements OnInit {

  products: any[] = [];
  categories: any = [];
  selectedFiles: File[] = [];
  imagePreviews: string[] = [];
  editingProductId: number | null = null;
  expandedProductId: number | null = null;
  currentImageIndexes: { [key: number]: number } = {};

  model: any = {
    name: '',
    description: '',
    price: null,
    quantity: null,
    imagePreviews: [],
    unitOfMeasurement: null,
    productImages: [],
    categoryId: null
  };

  constructor(
    private productService: ProductsService,
    private categoryService: CategoriesService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadProducts();

    this.categoryService.getAll()
      .subscribe(res => {
        this.categories = res;
        this.cdr.detectChanges();
      });
  }

  loadProducts() {
    this.productService.getMyProducts()
      .subscribe(res => {
        this.products = res;
        this.cdr.detectChanges();
      });
  }

  save() {
    
    // CREATE
    if (!this.editingProductId) {
    
      this.productService.create(this.model)
        .subscribe((createdProduct: any) => {
        
          // upload images
          this.uploadImages(createdProduct.id);
          this.toastr.success('Produs adăugat cu succes!');
        });
      
      return;
    }
  
    // UPDATE
    this.productService.update(
      this.editingProductId,
      this.model
    ).subscribe(() => {
    
      // upload new images
      this.uploadImages(this.editingProductId!);
      this.toastr.success('Produs actualizat cu succes!');
    });
  }

  afterSave() {

    this.resetForm();
    this.loadProducts();
  }

  edit(product: any) {
  
    this.editingProductId = product.id;

    this.model = {
      name: product.name,
      description: product.description,
      price: product.price,
      unitOfMeasurement: product.unitOfMeasurement,
      quantity: product.quantity,
      productImages: product.productImages,
      categoryId: product.categoryId
    };

      // smooth scroll to top
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

  delete(id: number) {
    if (!confirm('Doriți să ștergeți acest produs?'))
      return;

    this.productService.delete(id)
      .subscribe({

        next: () => {

          this.products =
            this.products.filter(p => p.id !== id);

          this.toastr.success(
            'Produs șters cu succes!'
          );
        },

        error: (err) => {
          console.error(err);
        }
      });
  }

  resetForm() {

    this.editingProductId = null;

    this.selectedFiles = [];
    this.imagePreviews = [];

    this.model = {
      name: '',
      description: '',
      price: 0,
      quantity: 0,
      unitOfMeasurement: '',
      categoryId: null
    };
  }

  onFileSelected(event: any) {

    const files = event.target.files;

    if (!files?.length)
      return;

    this.selectedFiles = [];
    this.imagePreviews = [];

    for (let file of files) {

      this.selectedFiles.push(file);

      const reader = new FileReader();

      reader.onload = (e: any) => {
        this.imagePreviews.push(e.target.result);
      };

      reader.readAsDataURL(file);
    }
  }

  uploadImages(productId: number) {
    // NO IMAGES
    if (!this.selectedFiles.length) {

      this.afterSave();
      return;
    }

    let uploaded = 0;

    for (let file of this.selectedFiles) {

      this.productService
        .uploadImage(productId, file)
        .subscribe({

          next: () => {

            uploaded++;

            // ALL IMAGES UPLOADED
            if (uploaded === this.selectedFiles.length) {

              this.loadProducts();

              this.resetForm();
            }
          },
          error: (err) => {
            console.error(err);
          }
        });
    }
  }

  deleteImage(id: number) {
   this.productService.deleteImage(id)
     .subscribe({

       next: () => {

         this.toastr.success('Imagine ștearsă');

         this.loadProducts();
       },

       error: (err) => {
         console.error(err);
       }
     });
  }

  toggleExpand(productId: number) {

    if (this.expandedProductId === productId) {

      this.expandedProductId = null;
      this.resetForm();
      return;
    }

    this.expandedProductId = productId;
  }

  nextImage(product: any) {
    if (!product.productImages?.length)
      return;

    if (!this.currentImageIndexes[product.id]) {
      this.currentImageIndexes[product.id] = 0;
    }

    this.currentImageIndexes[product.id] =
      (this.currentImageIndexes[product.id] + 1)
      % product.productImages.length;
  }

  prevImage(product: any) {
    if (!product.productImages?.length)
      return;

    if (!this.currentImageIndexes[product.id]) {
      this.currentImageIndexes[product.id] = 0;
    }

    this.currentImageIndexes[product.id]--;

    if (this.currentImageIndexes[product.id] < 0) {

      this.currentImageIndexes[product.id] =
        product.productImages.length - 1;
    }
  }

  getCurrentImage(product: any) {
    if (!product.productImages?.length)
      return null;

    const index =
      this.currentImageIndexes[product.id] || 0;

    return product.productImages[index];
  }
}