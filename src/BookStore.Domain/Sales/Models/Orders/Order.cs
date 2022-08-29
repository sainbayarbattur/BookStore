﻿namespace BookStore.Domain.Sales.Models.Orders;

using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Models;
using Customers;
using Exceptions;

public class Order : Entity<int>, IAggregateRoot
{
    private readonly HashSet<OrderedBook> orderedBooks;

    internal Order(Customer customer)
    {
        this.Customer = customer;

        this.Date = DateTime.UtcNow;
        this.Status = Status.Pending;

        this.orderedBooks = new HashSet<OrderedBook>();
    }

    private Order(DateTime date)
    {
        this.Date = date;

        this.Status = default!;
        this.Customer = default!;

        this.orderedBooks = new HashSet<OrderedBook>();
    }

    public DateTime Date { get; private set; }

    public Status Status { get; private set; }

    public Customer Customer { get; private set; }

    public IReadOnlyCollection<OrderedBook> OrderedBooks => this.orderedBooks.ToList().AsReadOnly();

    public Order MarkAsCancelled()
    {
        if (this.Status != Status.Pending)
        {
            throw new InvalidOrderException("Can't cancel an order that is not pending.");
        }

        this.Status = Status.Cancelled;

        return this;
    }

    public Order MarkAsCompleted()
    {
        if (this.Status != Status.Pending)
        {
            throw new InvalidOrderException("Can't mark as shipped an order that is not pending.");
        }

        this.Status = Status.Completed;

        return this;
    }

    public Order OrderBook(int bookId, int quantity)
    {
        this.orderedBooks.Add(new OrderedBook(bookId, quantity));

        // TODO: Fire quantity changed event
        // book.UpdateQuantity(quantity);

        return this;
    }
}