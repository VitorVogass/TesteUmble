import React, { useState } from 'react'
import ReactDOM from 'react-dom'

const Request = window.Request
const Headers = window.Headers
const fetch = window.fetch

class Api {
  async request (method, url, body) {
    if (body) {
      body = JSON.stringify(body)
    }

    const request = new Request('/api/' + url, {
      method: method,
      body: body,
      credentials: 'same-origin',
      headers: new Headers({
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      })
    })

    const resp = await fetch(request)
    if (!resp.ok && resp.status !== 400) {
      throw Error(resp.statusText)
    }

    const jsonResult = await resp.json()

    if (resp.status === 400) {
      jsonResult.requestStatus = 400
    }

    return jsonResult
  }

  async getDomain (domainOrIp) {
    return this.request('GET', `domain/${domainOrIp}`)
  }
}

const api = new Api()

function isValidDomain(q) {
  if (!q) return false
  q = q.trim().toLowerCase()
  const domain = /^(?!-)(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z]{2,}$/i
  return domain.test(q)
}

function parseWhois(whois) {
  if (!whois || typeof whois !== 'string') return {}

  function extract(pattern) {
    var match = whois.match(pattern)
    return match && match[1] ? match[1].trim() : null
  }

  return {
    cnpj: extract(/ownerid:\s*([0-9./-]+)/i),
    country: extract(/country:\s*([A-Z]{2})/i),
    email: extract(/e-mail:\s*([^\s]+)/i)
  }
}

function DomainResult({ data }) {
  const whois = parseWhois(data.whois)

  return (
    <div className="card mt-4">
      <div className="card-body">
        <h4>{data.name}</h4>

        <p><strong>IP:</strong> {data.ip}</p>
        <p><strong>Hospedado em:</strong> {data.hostedAt}</p>
        {whois.cnpj && (
          <p><strong>CNPJ:</strong> {whois.cnpj}</p>
        )}
        {whois.country && (
          <p><strong>País:</strong> {whois.country}</p>
        )}
        {whois.email && (
          <p><strong>Email:</strong> {whois.email}</p>
        )}
      </div>
    </div>
  )
}

function DomainLookup() {
  const [query, setQuery] = useState('')
  const [error, setError] = useState(null)
  const [loading, setLoading] = useState(false)
  const [result, setResult] = useState(null)

  async function onSearch() {
    setError(null)
    setResult(null)
    if (!isValidDomain(query)) {
      setError('Domínio inválido — inclua a extensão (ex: exemplo.com).')
      return
    }
    setLoading(true)
    try {
      const res = await api.getDomain(query.trim())
      setResult(res)
    } catch (e) {
      setError('Erro na requisição: ' + (e.message || e))
    } finally {
      setLoading(false)
    }
  }

  function onSubmit(e) {
    e.preventDefault()
    onSearch()
  }

  return (
    <div className="container py-4">
      <form className="row mb-3" onSubmit={onSubmit}>
        <div className="col-md-9">
          <input
            id="txt-search"
            className="form-control form-control-lg"
            placeholder="exemplo.com"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            disabled={loading}
          />
        </div>

        <div className="col-md-3">
          <button
            id="btn-search"
            type="submit"
            className="btn btn-success btn-block btn-lg"
            disabled={loading}
          >
            {loading ? 'Pesquisando...' : 'Pesquisar'}
          </button>
        </div>
      </form>

      {error && <div className="alert alert-danger">{error}</div>}
      {result && <DomainResult data={result} />}
    </div>
  )
}


document.addEventListener('DOMContentLoaded', () => {
  const mount = document.getElementById('domain-app')
  if (mount) {
    ReactDOM.render(<DomainLookup />, mount)
  }
})